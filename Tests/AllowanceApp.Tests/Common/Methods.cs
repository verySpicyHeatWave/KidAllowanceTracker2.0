using AllowanceApp.Core.Models;
using AllowanceApp.Core.Services;
using AllowanceApp.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AllowanceApp.Tests.Common
{

    public static class Methods
    {
        public const int EMPTY_DB = 0;
        public const int POPULATED_DB = 10;

        public static async Task<Dictionary<int, string>> StuffDatabaseWithRandomAccounts(IAccountServiceActor actor, int maxCount = POPULATED_DB)
        {
            Dictionary<int, string> test_accounts = [];
            for (int i = 1; i <= maxCount; i++)
            {
                test_accounts.Add(i, Guid.NewGuid().ToString());
                await actor.AddAccountAsync(test_accounts[i]);
            }
            return test_accounts;
        }
        public static Random GetRandomGenerator()
        {
            int epochSeed = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            return new Random(epochSeed);
        }

        public static Account GenericAccount()
        {
            var acct = new Account(Guid.NewGuid().ToString());
            var rng = GetRandomGenerator();
            int points = rng.Next(5, 50);
            for (int i = 0; i != points; i++)
            {
                var index = rng.Next(acct.Allowances.Count);
                acct.Allowances[index].Points++;
            }
            return acct;
        }

        public static AccountContext GetTestContext()
        {
            var options = new DbContextOptionsBuilder<AccountContext>()
                .UseSqlite("Data Source=:memory:")
                // .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            AccountContext context = new(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            return context;
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
    }
}