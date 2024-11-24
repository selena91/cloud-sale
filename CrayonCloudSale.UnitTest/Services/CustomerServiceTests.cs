using CrayonCloudSale.Infrastructure.Data.Models;
using CrayonCloudSale.Infrastructure.Services;
using CrayonCloudSale.Infrastructure.UnitOfWork;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq.Expressions;

namespace CrayonCloudSale.Tests
{
    [TestClass]
    public class CustomerServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private CustomerService _customerService;

        [TestInitialize]
        public void TestInitialize()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _customerService = new CustomerService(_unitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task GetIncludingAccountsAsync_ReturnsCustomerWithAccounts_WhenCustomerExists()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer
            {
                Id = customerId,
                Accounts = new List<Account>
                {
                    new Account { Id = 1, Name = "Account 1" },
                    new Account { Id = 2, Name = "Account 2" }
                }
            };

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetAsyncWithoutTracking(
                    It.IsAny<Expression<Func<Customer, bool>>>(),
                    null,
                    It.IsAny<Expression<Func<Customer, object>>>()))
                .ReturnsAsync(new List<Customer> { customer });

            // Act
            var result = await _customerService.GetIncludingAccountsAsync(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customerId, result?.Id);
            Assert.IsNotNull(result?.Accounts);
            Assert.AreEqual(2, result?.Accounts.Count);
        }

        [TestMethod]
        public async Task GetIncludingAccountsAsync_ReturnsNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = -1;

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetAsyncWithoutTracking(
                    It.IsAny<Expression<Func<Customer, bool>>>(),
                    null,
                    It.IsAny<Expression<Func<Customer, object>>>()))
                .ReturnsAsync(new List<Customer>());

            // Act
            var result = await _customerService.GetIncludingAccountsAsync(customerId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetIncludingAccountsAsync_IncludesAccountsInCustomer_WhenAccountsExist()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer
            {
                Id = customerId,
                Accounts = new List<Account>
                {
                    new Account { Id = 1, Name = "Account 1" },
                    new Account { Id = 2, Name = "Account 2" }
                }
            };

            _unitOfWorkMock.Setup(u => u.CustomerRepository.GetAsyncWithoutTracking(
                    It.IsAny<Expression<Func<Customer, bool>>>(),
                    null,
                    It.IsAny<Expression<Func<Customer, object>>>()))
                .ReturnsAsync(new List<Customer> { customer });

            // Act
            var result = await _customerService.GetIncludingAccountsAsync(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customerId, result?.Id);
            Assert.IsNotNull(result?.Accounts);
            Assert.AreEqual(2, result?.Accounts.Count);
            Assert.AreEqual("Account 1", result?.Accounts?.FirstOrDefault()?.Name);
        }
    }
}
