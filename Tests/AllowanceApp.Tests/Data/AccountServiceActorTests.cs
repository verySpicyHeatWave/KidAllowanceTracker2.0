using AllowanceApp.Core.Models;
using AllowanceApp.Core.Services;
using AllowanceApp.Data.Actors;
using AllowanceApp.Data.Contexts;
using AllowanceApp.Data.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AllowanceApp.Tests.Data
{
    public class AccountServiceActorTests
    {

        public static AccountContext GetTestContext()
        {
            var options = new DbContextOptionsBuilder<AccountContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AccountContext(options);
        }

        public static Mock<IAccountServiceActor> GetMockActor()
        {
            var mockActor = new Mock<IAccountServiceActor>();
            var expected = new Account("Brian") { AccountID = 1, Name = "Brian" };
            mockActor.Setup(a => a.AddAccountAsync("Brian")).ReturnsAsync(expected);

            return mockActor;
        }

        [Fact]
        public async Task GetAllAccountsAsync_ReturnsAccounts()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);

            string testName = "TestAccount";
            await actor.AddAccountAsync(testName);
            var accounts = await actor.GetAllAccountsAsync();

            Assert.Single(accounts);
            Assert.Equal(testName, accounts[0].Name);
        }

        [Fact]
        public async Task GetAllAccountsAsync_ThrowsIfDatabaseIsEmpty()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(actor.GetAllAccountsAsync);

            var strcomp = string.Compare(ex.Message, "No accounts found in database.", StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
        }

        [Fact]
        public async Task AddAccountAsync_NewAccountsIncrementByOne()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            var service = new AccountService(actor);
            Dictionary<int, string> test_accounts = [];
            for (int i = 1; i != 4; i++)
            {
                test_accounts.Add(i, Guid.NewGuid().ToString());
                await service.AddAccountAsync(test_accounts[i]);
            }

            var accounts = await service.GetAllAccountsAsync();
            foreach (var account in accounts)
            {
                Assert.Contains(account.AccountID, test_accounts.Keys);
                Assert.Equal(account.Name, test_accounts[account.AccountID]);
            }
        }
    }
}
