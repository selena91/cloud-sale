using CrayonCloudSale.Core;
using CrayonCloudSale.Infrastructure.Data.Models;
using CrayonCloudSale.Infrastructure.UnitOfWork;
using CrayonCloudSale.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq.Expressions;

namespace CrayonCloudSale.Tests
{
    [TestClass]
    public class CcpServiceTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<AzureConfiguration> _azureConfigurationMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private CcpService _ccpService;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _azureConfigurationMock = new Mock<AzureConfiguration>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _ccpService = new CcpService(_httpClientFactoryMock.Object, _azureConfigurationMock.Object, _unitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task GetSoftwareServices_ReturnsSoftwareList()
        {
            // Arrange
            var expectedSoftwareList = CcpService._softwareList;

            // Act
            var result = await _ccpService.GetSoftwareServices();

            // Assert
            Assert.AreEqual(expectedSoftwareList.Count, result.Count);
            Assert.IsTrue(result.Any(software => software.Name == "Microsoft Office 365"));
        }

        [TestMethod]
        public async Task OrderSoftware_ThrowsArgumentException_WhenAccountNotFound()
        {
            // Arrange
            var accountId = 1;
            var serviceName = "Microsoft Office 365";
            var quantity = 2;

            _unitOfWorkMock.Setup(u => u.AccountRepository.GetAsyncWithoutTracking(
                    It.IsAny<Expression<Func<Account, bool>>>(),
                    null,
                    It.IsAny<Expression<Func<Account, object>>>()))
                .ReturnsAsync(new List<Account>());

            // Act
            var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _ccpService.OrderSoftware(accountId, serviceName, quantity));

            // Assert
            Assert.AreEqual($"Account with id {accountId} not found.", exception.Message);
        }

        [TestMethod]
        public async Task OrderSoftware_ThrowsInvalidCastException_WhenSoftwareAlreadyPurchased()
        {
            // Arrange
            var accountId = 1;
            var serviceName = "Microsoft Office 365";
            var quantity = 2;

            var existingAccount = new Account
            {
                Id = accountId,
                PurchasedSoftwares = new List<PurchasedSoftware>
                {
                    new PurchasedSoftware { Name = serviceName, Quantity = 1, State = State.Active }
                }
            };

            _unitOfWorkMock.Setup(u => u.AccountRepository.GetAsyncWithoutTracking(
                    It.IsAny<Expression<Func<Account, bool>>>(),
                    null,
                    It.IsAny<Expression<Func<Account, object>>>()))
                .ReturnsAsync(new List<Account> { existingAccount });

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<InvalidCastException>(() => _ccpService.OrderSoftware(accountId, serviceName, quantity));
            Assert.AreEqual($"Account with id {accountId} already purchased license for {serviceName}", exception.Message);
        }

        [TestMethod]
        public async Task OrderSoftware_AddsPurchasedSoftware_WhenValid()
        {
            // Arrange
            var accountId = 1;
            var serviceName = "Zoom";
            var quantity = 1;

            var account = new Account
            {
                Id = accountId,
                PurchasedSoftwares = new List<PurchasedSoftware>()
            };

            _unitOfWorkMock.Setup(u => u.AccountRepository.GetAsyncWithoutTracking(
                    It.IsAny<Expression<Func<Account, bool>>>(),
                    null,
                    It.IsAny<Expression<Func<Account, object>>>()))
                .ReturnsAsync(new List<Account> { account });

            _unitOfWorkMock.Setup(u => u.AccountRepository.Update(It.IsAny<Account>()));

            // Act
            await _ccpService.OrderSoftware(accountId, serviceName, quantity);

            // Assert
            Assert.AreEqual(1, account.PurchasedSoftwares.Count);
            Assert.AreEqual(serviceName, account.PurchasedSoftwares.First().Name);
            _unitOfWorkMock.Verify(u => u.AccountRepository.Update(It.IsAny<Account>()), Times.Once);
        }
    }
}
