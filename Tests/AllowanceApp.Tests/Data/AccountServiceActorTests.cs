using System.Data.Common;
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

        public static List<string> GetDefaultBehaviorStrings()
        {
            return
            [   "BaseAllowance",
                "GoodBehavior",
                "Homework",
                "Chores",
                "BadBehavior",
                "GradeA",
                "GradeB",
                "GradeC",
                "GradeD",
                "GradeF"
            ];
        }

        protected static async Task<Dictionary<int, string>> StuffDatabaseWithRandomAccounts(IAccountServiceActor actor, int maxCount = 10)
        {
            Dictionary<int, string> test_accounts = [];
            for (int i = 1; i <= maxCount; i++)
            {
                test_accounts.Add(i, Guid.NewGuid().ToString());
                await actor.AddAccountAsync(test_accounts[i]);
            }
            return test_accounts;
        }

        // BCOBB: This is for a different test file, for the AccountService tests
        // public static Mock<IAccountServiceActor> GetMockActor()
        // {
        //     var mockActor = new Mock<IAccountServiceActor>();
        //     var expected = new Account("Brian") { AccountID = 1, Name = "Brian" };
        //     mockActor.Setup(a => a.AddAccountAsync("Brian")).ReturnsAsync(expected);

        //     return mockActor;
        // }

        [Fact]
        public async Task AddAccountAsync_NewAccountsIncrementByOne()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            var test_accounts = await StuffDatabaseWithRandomAccounts(actor, 3);

            var accounts = await actor.GetAllAccountsAsync();
            foreach (var account in accounts)
            {
                Assert.Contains(account.AccountID, test_accounts.Keys);
                Assert.Equal(account.Name, test_accounts[account.AccountID]);
            }
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
        public async Task GetAccountAsync_ReturnsAccountWithID()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            var test_accounts = await StuffDatabaseWithRandomAccounts(actor);
            int epochSeed = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var id = new Random(epochSeed).Next(test_accounts.Count);
            var account = await actor.GetAccountAsync(id);

            var strcomp = string.Compare(test_accounts[id], account.Name, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
            Assert.Equal(id, account.AccountID);
        }


        [Fact]
        public async Task GetAccountAsync_ThrowsIfEmptyDB()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            int epochSeed = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var id = new Random(epochSeed).Next(100);
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () => await actor.GetAccountAsync(id));

            var strcomp = string.Compare(ex.Message, $"No account found with ID number {id}", StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
        }


        [Fact]
        public async Task GetAccountAsync_ThrowsIfAccountNotFound()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);

            _ = await StuffDatabaseWithRandomAccounts(actor);

            int epochSeed = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var id = new Random(epochSeed).Next(4, 100);
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () => await actor.GetAccountAsync(id));

            var strcomp = string.Compare(ex.Message, $"No account found with ID number {id}", StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
        }


        [Fact]
        public async Task GetAllowancePointAsync_ReturnsPointWithCorrectID()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            var test_accounts = await StuffDatabaseWithRandomAccounts(actor);
            var defaultPointStrings = GetDefaultBehaviorStrings();

            int epochSeed = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var index = new Random(epochSeed).Next(defaultPointStrings.Count);
            var id = new Random(epochSeed).Next(test_accounts.Count);
            
            var point = await actor.GetAllowancePointAsync(id, defaultPointStrings[index]);

            var strcomp = string.Compare(defaultPointStrings[index], point.Category, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
            Assert.Equal(id, point.AccountID);
        }


        [Fact]
        public async Task GetAllowancePointAsync_ThrowsIfEmptyDB()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            var defaultPointStrings = GetDefaultBehaviorStrings();

            int epochSeed = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var index = new Random(epochSeed).Next(defaultPointStrings.Count);
            var id = new Random(epochSeed).Next(100);
            
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () => await actor.GetAllowancePointAsync(id, defaultPointStrings[index]));

            var strcomp = string.Compare(ex.Message, $"No account found with ID number {id}", StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
        }


        [Fact]
        public async Task GetAllowancePointAsync_ThrowsIfAccountNotFound()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            var test_accounts = await StuffDatabaseWithRandomAccounts(actor, 3);
            var defaultPointStrings = GetDefaultBehaviorStrings();

            int epochSeed = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var index = new Random(epochSeed).Next(defaultPointStrings.Count);
            var id = new Random(epochSeed).Next(4, 100);
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () => await actor.GetAllowancePointAsync(id, defaultPointStrings[index]));

            var strcomp = string.Compare(ex.Message, $"No account found with ID number {id}", StringComparison.OrdinalIgnoreCase);
            Assert.Equal(0, strcomp);
        }


        [Fact]
        public async Task GetAllowancePointAsync_ThrowsIfPointNameNotFound()
        {
            using var context = GetTestContext();
            var actor = new AccountServiceActor(context);
            var defaultPointStrings = GetDefaultBehaviorStrings();

            await actor.AddAccountAsync(Guid.NewGuid().ToString());

            var id = 1;
            var badCategory = Guid.NewGuid().ToString();
            var ex = await Assert.ThrowsAsync<DataNotFoundException>(async () => await actor.GetAllowancePointAsync(id, badCategory));

            var strcomp = ex.Message.Contains($"Could not find allowance type {badCategory}", StringComparison.OrdinalIgnoreCase);
            Assert.True(strcomp);
        }
    }
}
