using AllowanceApp.Data.Actors;
using AllowanceApp.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AllowanceApp.Tests.Data
{
    public class AccountServiceActorTests
    {
        [Fact]
        public async Task GetAllAccountsAsync_ReturnsAccounts()
        {
            string testName = "TestAccount";

            var options = new DbContextOptionsBuilder<AccountContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            using var context = new AccountContext(options);
            var actor = new AccountServiceActor(context);
            await actor.AddAccountAsync(testName);
            context.SaveChanges();
            var accounts = await actor.GetAllAccountsAsync();

            var index = -1;
            var accountID = -1;
            for (int i = 0; i < accounts.Count; i++)
            {
                if (String.Compare(testName, accounts[i].Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    index = i;
                    accountID = accounts[i].AccountID;
                    break;
                }
            }
            Assert.NotEqual(-1, index);
        }
    }
}
