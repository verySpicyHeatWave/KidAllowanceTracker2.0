using AllowanceApp.Core.Models;
using AllowanceApp.Data.Actors;
using AllowanceApp.Data.Exceptions;
using AllowanceApp.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace AllowanceApp.Tests.Data
{
    public class AccountServiceActorTests
    {
        #region "AddAccountAsync"
        [Fact]
        public async Task AddAccountAsync_NewAccountsIncrementByOne()
        {
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            var test_accounts = await Methods.StuffDatabaseWithRandomAccounts(actor, 3);

            var accounts = await actor.GetAllAccountsAsync();
            foreach (var account in accounts)
            {
                Assert.Contains(account.AccountID, test_accounts.Keys);
                Assert.Equal(account.Name, test_accounts[account.AccountID]);
            }
        }

        [Fact]
        public async Task AddAccountAsync_RefuseRepeatNames()
        {
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            string name = Guid.NewGuid().ToString();
            await actor.AddAccountAsync(name);
            var ex = await Assert.ThrowsAsync<DbUpdateException>(async () => await actor.AddAccountAsync(name));         
        }
        #endregion


        #region "GetAllAccountsAsync"
        [Fact]
        public async Task GetAllAccountsAsync_ReturnsAccounts()
        {
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);

            string testName = Guid.NewGuid().ToString();
            await actor.AddAccountAsync(testName);
            var accounts = await actor.GetAllAccountsAsync();

            Assert.Single(accounts);
            Assert.Equal(testName, accounts[0].Name);
        }

        [Fact]
        public async Task GetAllAccountsAsync_ThrowsIfDatabaseIsEmpty()
        {
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(actor.GetAllAccountsAsync);

            Assert.True(string.Equals(ex.Message, "No accounts found in database.", StringComparison.OrdinalIgnoreCase));
        }
        #endregion


        #region "GetAccountAsync"

        [Fact]
        public async Task GetAccountAsync_ReturnsAccountWithID()
        {
            var rng = Methods.GetRandomGenerator();
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            var test_accounts = await Methods.StuffDatabaseWithRandomAccounts(actor);

            var id = rng.Next(1, test_accounts.Count);
            var account = await actor.GetAccountAsync(id);

            var strcomp = string.Compare(test_accounts[id], account.Name, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
            Assert.Equal(id, account.AccountID);
        }

        [Theory]
        [InlineData(Methods.EMPTY_DB)]
        [InlineData(Methods.POPULATED_DB)]
        public async Task GetAccountAsync_Throws(int AccountsInDB)
        {
            var rng = Methods.GetRandomGenerator();
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);

            int id = 1;
            if (AccountsInDB > 0)
            {
                var test_accounts = await Methods.StuffDatabaseWithRandomAccounts(actor, AccountsInDB);
                id = rng.Next(AccountsInDB + 1, 100);
            }

            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () =>
                await actor.GetAccountAsync(id));

            Assert.True(string.Equals(ex.Message, $"No account found with ID number {id}", StringComparison.OrdinalIgnoreCase));
        }
        #endregion


        #region "GetAllowancePointAsync"
        [Fact]
        public async Task GetAllowancePointAsync_ReturnsPointWithCorrectID()
        {
            var rng = Methods.GetRandomGenerator();

            
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            var test_accounts = await Methods.StuffDatabaseWithRandomAccounts(actor);
            var category = Methods.GetRandomBehaviorString(rng);

            var id = rng.Next(1, test_accounts.Count);

            var point = await actor.GetAllowancePointAsync(id, category);

            var strcomp = string.Compare(category, point.Category, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
            Assert.Equal(id, point.AccountID);
        }

        [Theory]
        [InlineData(Methods.EMPTY_DB)]
        [InlineData(Methods.POPULATED_DB)]
        public async Task GetAllowancePointAsync_ThrowsOnAccountGet(int AccountsInDB)
        {
            var rng = Methods.GetRandomGenerator();
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            var category = Methods.GetRandomBehaviorString(rng);

            int id = 1;
            if (AccountsInDB > 0)
            {
                var test_accounts = await Methods.StuffDatabaseWithRandomAccounts(actor, AccountsInDB);
                id = rng.Next(AccountsInDB + 1, 100);
            }

            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () =>
                await actor.GetAllowancePointAsync(id, category));

            Assert.True(string.Equals(ex.Message, $"No account found with ID number {id}", StringComparison.OrdinalIgnoreCase));
        }


        [Fact]
        public async Task GetAllowancePointAsync_ThrowsIfPointNameNotFound()
        {
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);

            await actor.AddAccountAsync(Guid.NewGuid().ToString());

            var id = 1;
            var badCategory = Guid.NewGuid().ToString();
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () => await actor.GetAllowancePointAsync(id, badCategory));

            var strcomp = ex.Message.Contains($"Could not find allowance type {badCategory}", StringComparison.OrdinalIgnoreCase);
            Assert.True(strcomp);
        }
        #endregion


        #region "SinglePointAdjustAsync"
        [Theory]
        [InlineData(PointOperation.Increment)]
        [InlineData(PointOperation.Decrement)]
        public async Task SinglePointAdjustAsync_IncOrDecByOne(PointOperation operation)
        {
            var rng = Methods.GetRandomGenerator();
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            var acct = await actor.AddAccountAsync(Guid.NewGuid().ToString());

            var category = Methods.GetRandomBehaviorString(rng);
            int id = 1;

            var random_add = rng.Next(2, 10);
            for (int i = 0; i != random_add; i++)
            {
                await actor.SinglePointAdjustAsync(id, category, PointOperation.Increment);
            }

            var oldValue = (await actor.GetAllowancePointAsync(id, category)).Points;
            var newPoint = await actor.SinglePointAdjustAsync(id, category, operation);

            Assert.Equal(oldValue + (int)operation, newPoint.Points);
        }
        #endregion


        #region "UpdateAllowancePriceAsync"
        [Fact]
        public async Task UpdateAllowancePriceAsync_UpdatesToWhatever()
        {
            var rng = Methods.GetRandomGenerator();
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            _ = await actor.AddAccountAsync(Guid.NewGuid().ToString());

            var category = Methods.GetRandomBehaviorString(rng);
            int id = 1;

            var random_add = rng.Next(0, 500);

            for (int i = 0; i != 5; i++)
            {
                if (i % 2 == 1) { random_add *= -1; }
                var newPoint = await actor.UpdateAllowancePriceAsync(id, category, random_add);
                Assert.Equal(random_add, newPoint.Price);
                random_add = rng.Next(0, 500);
            }
        }
        #endregion


        #region "PayAllowanceAsync"
        [Fact]
        public async Task PayAllowanceAsync_PaysOutCorrectly()
        {
            var rng = Methods.GetRandomGenerator();
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            _ = await actor.AddAccountAsync(Guid.NewGuid().ToString());

            var category = Methods.GetDefaultBehaviorStrings();
            var index = rng.Next(category.Count);
            int id = 1;

            for (int i = 0; i != 10; i++)
            {
                await actor.SinglePointAdjustAsync(id, category[index], PointOperation.Increment);
                index = rng.Next(category.Count);
            }
            Account unpaidAcct = await actor.GetAccountAsync(id);
            Assert.Empty(unpaidAcct.Transactions);
            var total = unpaidAcct.AllowanceBalance;
            await actor.PayAllowanceAsync(id);
            var paidAcct = await actor.GetAccountAsync(id);
            Assert.Single(paidAcct.Transactions);
            Assert.Equal(total, paidAcct.Transactions[0].Amount);
        }
        #endregion


        #region "ApplyTransactionAsync"
        [Theory]
        [InlineData(TransactionType.Deposit)]
        [InlineData(TransactionType.Withdraw)]
        public async Task ApplyTransactionAsync_OperatesCorrectlyAsync(TransactionType action)
        {
            var rng = Methods.GetRandomGenerator();
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            _ = await actor.AddAccountAsync(Guid.NewGuid().ToString());

            int id = 1;
            int value = rng.Next(1000, 10000);
            int oldBalance = value;
            string description = Guid.NewGuid().ToString();

            await actor.RequestTransactionAsync(id, value, TransactionType.Deposit, description);

            value = rng.Next(100, 500);
            description = Guid.NewGuid().ToString();

            await actor.RequestTransactionAsync(id, value, action, description);
            Account acct = await actor.GetAccountAsync(id);
            Assert.Equal(oldBalance + ((int)action * value), acct.Balance);
        }
        #endregion


        #region "DeleteAccountAsync"

        [Fact]
        public async Task DeleteAccountAsync_OperatesCorrectly()
        {
            var rng = Methods.GetRandomGenerator();
            using var context = Methods.GetTestContext();
            var actor = new AccountServiceActor(context);
            var test_accounts = await Methods.StuffDatabaseWithRandomAccounts(actor, 20);

            int id = rng.Next(1, 20);

            Account acct = await actor.GetAccountAsync(id);
            await actor.DeleteAccountAsync(id);
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () => await actor.GetAccountAsync(id));

            Assert.True(string.Equals(ex.Message, $"No account found with ID number {id}", StringComparison.OrdinalIgnoreCase));
        }
        
        #endregion
    }
}
