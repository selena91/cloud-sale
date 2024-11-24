using CrayonCloudSale.Infrastructure.Data.Models;
using CrayonCloudSale.Infrastructure.Services;
using CrayonCloudSale.Infrastructure.UnitOfWork;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq.Expressions;

namespace CrayonCloudSale.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private AccountService _accountService;

        [TestInitialize]
        public void TestInitialize()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _accountService = new AccountService(_unitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task GetWithPurchasedSoftwares_ReturnsAccountWithPurchasedSoftwares_WhenAccountExists()
        {
            // Arrange
            var accountId = 1;
            var account = new Account
            {
                Id = accountId,
                PurchasedSoftwares = new List<PurchasedSoftware>
                {
                    new PurchasedSoftware { Id = 1, Name = "Software 1", Quantity = 2, State = State.Active },
                    new PurchasedSoftware { Id = 2, Name = "Software 2", Quantity = 1, State = State.Active }
                }
            };

            _unitOfWorkMock.Setup(u => u.AccountRepository.GetAsyncWithoutTracking(
                   It.IsAny<Expression<Func<Account, bool>>>(),
                   null,
                   It.IsAny<Expression<Func<Account, object>>>()))
                .ReturnsAsync(new List<Account> { account });

            // Act
            var result = await _accountService.GetWithPurchasedSoftwares(accountId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(accountId, result?.Id);
            Assert.IsNotNull(result?.PurchasedSoftwares);
            Assert.AreEqual(2, result?.PurchasedSoftwares.Count);
            Assert.AreEqual("Software 1", result?.PurchasedSoftwares?.FirstOrDefault()?.Name);
        }

        [TestMethod]
        public async Task GetWithPurchasedSoftwares_ReturnsNull_WhenAccountDoesNotExist()
        {
            // Arrange
            var accountId = -1; 

            _unitOfWorkMock.Setup(u => u.AccountRepository.GetAsyncWithoutTracking(
                  It.IsAny<Expression<Func<Account, bool>>>(),
                  null,
                  It.IsAny<Expression<Func<Account, object>>>()))
               .ReturnsAsync(new List<Account>());

            // Act
            var result = await _accountService.GetWithPurchasedSoftwares(accountId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetWithPurchasedSoftwares_ReturnsAccountWithEmptyPurchasedSoftwares_WhenAccountExistsWithNoPurchasedSoftwares()
        {
            // Arrange
            var accountId = 1;
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

            // Act
            var result = await _accountService.GetWithPurchasedSoftwares(accountId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(accountId, result?.Id);
            Assert.IsNotNull(result?.PurchasedSoftwares);
            Assert.AreEqual(0, result?.PurchasedSoftwares.Count);
        }
    }
}
