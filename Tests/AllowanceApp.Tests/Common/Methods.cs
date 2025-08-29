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

        public static Account GetLivelyAccount(string Name)
        {
            var rng = Methods.GetRandomGenerator();
            var transactions = rng.Next(2, 10);
            int amount;

            var account = new Account(Name);

            for (int i = 0; i != transactions; i++)
            {
                amount = rng.Next(-5000, 5000);
                account.Transactions.Add(new Transaction()
                {
                    Description = Guid.NewGuid().ToString(),
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    Amount = amount
                });
            }

            foreach (var point in account.Allowances)
            {
                point.Points = rng.Next(1, 10);
            }

            account.Balance = rng.Next(3000, 5000);

            return account;
        }

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

        public static string GetRandomBehaviorString(Random? rng = null)
        {
            rng ??= GetRandomGenerator();
            var categories = GetDefaultBehaviorStrings();
            var index = rng.Next(categories.Count);
            return categories[index];
        }

        public static AccountService GetAccountServiceWithMock()
        {

            var mockActor = new Mock<IAccountServiceActor>();
            var expected = new Account("TestAccount");

            mockActor
                .Setup(a => a.AddAccountAsync(It.IsAny<string>()))
                .ReturnsAsync((string name) => new Account(name)); ;

            mockActor
                .Setup(a => a.GetAllAccountsAsync())
                .ReturnsAsync([
                    new Account(Guid.NewGuid().ToString()),
                    new Account(Guid.NewGuid().ToString()),
                    new Account(Guid.NewGuid().ToString()),
                    new Account(Guid.NewGuid().ToString()),
                    new Account(Guid.NewGuid().ToString())
                ]);

            mockActor
                .Setup(a => a.GetAccountAsync(It.Is<int>(id => id > 0)))
                .ReturnsAsync((int id) => new Account(Guid.NewGuid().ToString()) { AccountID = id });

            mockActor
                .Setup(a => a.GetAllowancePointAsync(It.Is<int>(id => id > 0), It.IsAny<string>()))
                .ReturnsAsync((int id, string category) => new AllowancePoint(category, 1) { AccountID = id });

            mockActor
                .Setup(a => a.SinglePointAdjustAsync(It.Is<int>(id => id > 0), It.IsAny<string>(), It.IsAny<PointOperation>()))
                .ReturnsAsync((int id, string category, PointOperation operation) => new AllowancePoint(category, 1) { AccountID = id, Points = 1 });

            mockActor
                .Setup(a => a.UpdateAllowancePriceAsync(It.Is<int>(id => id > 0), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((int id, string category, int amount) => new AllowancePoint(category, amount) { AccountID = id });

            mockActor
                .Setup(a => a.PayAllowanceAsync(It.Is<int>(id => id > 0)))
                .ReturnsAsync((int id) =>
                {
                    var rng = GetRandomGenerator();
                    Account expected = new(Guid.NewGuid().ToString()) { AccountID = id };
                    expected.PayAllowanceToAccount();
                    expected.Balance = rng.Next();
                    return expected;
                });

            mockActor
                .Setup(a => a.ApplyTransactionAsync(It.Is<int>(id => id > 0), It.Is<int>(d => d > 0), It.IsAny<TransactionType>(), It.IsAny<string>()))
                .ReturnsAsync((int id, int amount, TransactionType action, string description) =>
                {
                    var rng = GetRandomGenerator();
                    Account expected = new(Guid.NewGuid().ToString()) { AccountID = id };
                    expected.ApplyTransaction(amount, action, description);
                    expected.Transactions[0].TransactionID = rng.Next();
                    return expected;
                });

            mockActor
                .Setup(a => a.DeleteAccountAsync(It.Is<int>(id => id > 0)))
                .ReturnsAsync((int id) => $"Account_{id}");

            return new AccountService(mockActor.Object);
        }
    }
}