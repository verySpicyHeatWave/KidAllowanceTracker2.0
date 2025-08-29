using AllowanceApp.Core.Models;
using AllowanceApp.Core.Services;
using AllowanceApp.Tests.Common;

namespace AllowanceApp.Tests.Core
{
    public class AccountServiceTests
    {
        [Fact]
        public static async Task AddAccountAsyncTest()
        {
            AccountService service = Methods.GetAccountServiceWithMock();
            string name = Guid.NewGuid().ToString();
            Account result = await service.AddAccountAsync(name);
            Assert.True(string.Equals(result.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static async Task GetAllAccountsAsyncTest()
        {
            AccountService service = Methods.GetAccountServiceWithMock();
            List<Account> result = await service.GetAllAccountsAsync();
            Assert.NotEmpty(result);
        }

        [Fact]
        public static async Task GetAccountAsyncTest()
        {
            Random rng = Methods.GetRandomGenerator();
            int id = rng.Next();
            AccountService service = Methods.GetAccountServiceWithMock();
            Account result = await service.GetAccountAsync(id);
            Assert.Equal(id, result.AccountID);
        }

        [Fact]
        public static async Task GetAllowancePointAsyncTest()
        {
            Random rng = Methods.GetRandomGenerator();
            int id = rng.Next();
            string category = Guid.NewGuid().ToString();
            AccountService service = Methods.GetAccountServiceWithMock();
            AllowancePoint result = await service.GetAllowancePointAsync(id, category);
            Assert.Equal(id, result.AccountID);
            Assert.True(string.Equals(result.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static async Task SinglePointAdjustAsyncTest()
        {
            Random rng = Methods.GetRandomGenerator();
            int id = rng.Next();
            string category = Guid.NewGuid().ToString();
            AccountService service = Methods.GetAccountServiceWithMock();
            AllowancePoint result = await service.SinglePointAdjustAsync(id, category, PointOperation.Increment);
            Assert.Equal(id, result.AccountID);
            Assert.True(string.Equals(result.Category, category, StringComparison.OrdinalIgnoreCase));
            Assert.Equal(1, result.Points);
        }

        [Fact]
        public static async Task UpdateAllowancePriceAsyncTest()
        {
            Random rng = Methods.GetRandomGenerator();
            int id = rng.Next();
            string category = Guid.NewGuid().ToString();
            int amount = rng.Next();
            AccountService service = Methods.GetAccountServiceWithMock();
            AllowancePoint result = await service.UpdateAllowancePriceAsync(id, category, amount);
            Assert.Equal(id, result.AccountID);
            Assert.True(string.Equals(result.Category, category, StringComparison.OrdinalIgnoreCase));
            Assert.Equal(amount, result.Price);
        }

        [Fact]
        public static async Task PayAllowanceAsyncTest()
        {
            Random rng = Methods.GetRandomGenerator();
            int id = rng.Next();
            string category = Guid.NewGuid().ToString();
            int amount = rng.Next();
            AccountService service = Methods.GetAccountServiceWithMock();
            Account result = await service.PayAllowanceAsync(id);
            Assert.Equal(id, result.AccountID);
            Assert.Equal(0, result.PointsBalance);
            Assert.True(result.Balance > 0);
        }

        [Fact]
        public static async Task ApplyTransactionAsyncTest()
        {
            Random rng = Methods.GetRandomGenerator();
            int id = rng.Next();
            string description = Guid.NewGuid().ToString();
            int amount = rng.Next();
            AccountService service = Methods.GetAccountServiceWithMock();
            Account result = await service.ApplyTransactionAsync(id, amount, TransactionType.Deposit, description);
            Assert.Equal(amount, result.Balance);
            Assert.Single(result.Transactions);
            Transaction transaction = result.Transactions[0];
            Assert.Equal(id, transaction.AccountID);
            Assert.Equal(amount, transaction.Amount);
            Assert.True(string.Equals(description, transaction.Description, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static async Task DeleteAccountAsyncTest()
        {
            Random rng = Methods.GetRandomGenerator();
            int id = rng.Next();
            string description = Guid.NewGuid().ToString();
            int amount = rng.Next();
            AccountService service = Methods.GetAccountServiceWithMock();
            string result = await service.DeleteAccountAsync(id);
            Assert.True(string.Equals(result, $"Account_{id}", StringComparison.OrdinalIgnoreCase));
        }
    }
}
