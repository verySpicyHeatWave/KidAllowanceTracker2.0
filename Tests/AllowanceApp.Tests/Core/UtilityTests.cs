using System.Diagnostics;
using AllowanceApp.Core.Models;
using AllowanceApp.Tests.Common;

namespace AllowanceApp.Tests.Core
{
    public class TransactionUtilityTests
    {
        #region RequestTransaction
        [Theory]
        [InlineData(TransactionType.Deposit)]
        [InlineData(TransactionType.Withdraw)]
        public void RequestTransaction_OperatesCorrectly(TransactionType action)
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            // Withdraw amount should be less than the account balance
            var amount = rng.Next(1, 500);
            acct.Balance = 1000;
            var oldBalance = acct.Balance;

            // Apply transaction, then find that exact transaction item
            var description = Guid.NewGuid().ToString();
            acct.RequestTransaction(amount, action, description);
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            int newBalance = oldBalance + (amount * (int)action);

            // Transaction shouldn't be null
            Assert.NotNull(transaction);

            // Transaction amount should equal the amount requested
            Assert.Equal(amount * (int)action, transaction.Amount);

            // Transaction should stil be pending
            Assert.Equal(ApprovalStatus.Pending, transaction.Status);

            //The balance should not have changed
            Assert.Equal(oldBalance, acct.Balance);
        }

        [Fact]
        public void RequestTransaction_CantWithdrawFromEmptyBalance()
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            // Balance should be zero
            acct.Balance = 0;
            var amount = rng.Next(1, 500);

            // Apply transaction, then find that exact transaction item
            var description = Guid.NewGuid().ToString();
            acct.RequestTransaction(amount, TransactionType.Withdraw, description);
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            // There should be no transaction activity
            Assert.Null(transaction);
        }

        [Theory]
        [InlineData(TransactionType.Deposit)]
        [InlineData(TransactionType.Withdraw)]
        public void RequestTransaction_CantTransactNothing(TransactionType action)
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            // Requested amount should be zero
            acct.Balance = 1000;
            var amount = 0;

            // Apply transaction, then find that exact transaction item
            var description = Guid.NewGuid().ToString();
            acct.RequestTransaction(amount, action, description);
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            // There should be no transaction activity
            Assert.Null(transaction);
        }

        [Theory]
        [InlineData(TransactionType.Deposit)]
        [InlineData(TransactionType.Withdraw)]
        public void RequestTransaction_CantTransactNegative(TransactionType action)
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            // Requested amount should be negative
            acct.Balance = 1000;
            var amount = rng.Next(-500, -1);

            // Apply transaction, then find that exact transaction item
            var description = Guid.NewGuid().ToString();
            acct.RequestTransaction(amount, action, description);
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            // There should be no transaction activity
            Assert.Null(transaction);
        }
        #endregion


        #region ApproveTransaction
        [Theory]
        [InlineData(TransactionType.Deposit)]
        [InlineData(TransactionType.Withdraw)]
        public void ApproveTransaction_OperatesCorrectly(TransactionType action)
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            // Withdraw amount should be less than the account balance
            var amount = rng.Next(1, 500);
            acct.Balance = 1000;
            var oldBalance = acct.Balance;

            // Apply transaction, then find that exact transaction item
            var description = Guid.NewGuid().ToString();
            acct.RequestTransaction(amount, action, description);
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            int newBalance = oldBalance + (amount * (int)action);

            // Transaction shouldn't be null--then we will use the transaction ID to approve
            Assert.NotNull(transaction);
            Assert.Equal(ApprovalStatus.Pending, transaction.Status);
            Assert.Equal(amount * (int)action, transaction.Amount);
            acct.ApproveTransaction(transaction.TransactionID);

            // Reacquire the updated transaction object
            transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            // Transaction shouldn't be null
            Assert.NotNull(transaction);

            // Transaction amount should equal the amount requested
            Assert.Equal(amount * (int)action, transaction.Amount);
            Assert.Equal(ApprovalStatus.Approved, transaction.Status);

            //The balance should have changed
            Assert.Equal(newBalance, acct.Balance);
        }

        [Fact]
        public void ApproveTransaction_OverdraftWithdrawsEntireBalance()
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            // Withdraw amount should be more than the account balance
            var amount = rng.Next(101, 500);
            acct.Balance = rng.Next(1, 100);
            var oldBalance = acct.Balance;

            // Apply transaction, then find that exact transaction item
            var description = Guid.NewGuid().ToString();
            acct.RequestTransaction(amount, TransactionType.Withdraw, description);
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            // Transaction shouldn't be null--then we will use the transaction ID to approve
            Assert.NotNull(transaction);
            acct.ApproveTransaction(transaction.TransactionID);

            transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            // Transaction shouldn't be null
            Assert.NotNull(transaction);

            // The transaction amount should equal the balance, NOT the amount requested
            Assert.Equal(-oldBalance, transaction.Amount);

            //The balance should be empty
            Assert.Equal(0, acct.Balance);
        }

        [Fact]
        public void ApproveTransaction_CanDepositToEmptyBalance()
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            // Balance should be zero
            acct.Balance = 0;
            var amount = rng.Next(1, 500);

            // Apply transaction, then find that exact transaction item
            var description = Guid.NewGuid().ToString();
            acct.RequestTransaction(amount, TransactionType.Deposit, description);
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            // Transaction shouldn't be null--then we will use the transaction ID to approve
            Assert.NotNull(transaction);
            acct.ApproveTransaction(transaction.TransactionID);

            transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            // Transaction shouldn't be null
            Assert.NotNull(transaction);

            // Transaction amount should equal the amount requested
            Assert.Equal(amount, transaction.Amount);

            //The balance should be whatever the requested amount was
            Assert.Equal(amount, acct.Balance);
        }

        [Fact]
        public void ApproveTransaction_ApprovingNonexistantTransactionDoesNothing()
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            var nextID = rng.Next();
            acct.ApproveTransaction(nextID);

            Assert.Empty(acct.Transactions);
        }
        #endregion


        #region DeclineTransaction
        [Theory]
        [InlineData(TransactionType.Deposit)]
        [InlineData(TransactionType.Withdraw)]
        public void DeclineTransaction_OperatesCorrectly(TransactionType action)
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            // Withdraw amount should be less than the account balance
            var amount = rng.Next(1, 500);
            acct.Balance = 1000;
            var oldBalance = acct.Balance;

            // Apply transaction, then find that exact transaction item
            var description = Guid.NewGuid().ToString();
            acct.RequestTransaction(amount, action, description);
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            int newBalance = oldBalance + (amount * (int)action);

            // Transaction shouldn't be null--then we will use the transaction ID to approve
            Assert.NotNull(transaction);
            Assert.Equal(ApprovalStatus.Pending, transaction.Status);
            Assert.Equal(amount * (int)action, transaction.Amount);
            acct.DeclineTransaction(transaction.TransactionID);

            // Reacquire the updated transaction object
            transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(transaction);

            // Transaction amount should equal the amount requested
            Assert.Equal(amount * (int)action, transaction.Amount);
            Assert.Equal(ApprovalStatus.Declined, transaction.Status);

            //The balance should not have changed
            Assert.Equal(oldBalance, acct.Balance);
        }

        [Fact]
        public void DeclineTransaction_DecliningNonexistantTransactionDoesNothing()
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            var nextID = rng.Next();
            acct.DeclineTransaction(nextID);

            Assert.Empty(acct.Transactions);
        }
        #endregion


        #region PayAllowanceToAccount
        [Fact]
        public void PayAllowanceToAccount_RequestsTransaction()
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();

            acct.Balance = rng.Next(0, 5000);
            var oldBalance = acct.Balance;
            var allowance = acct.AllowanceBalance;
            var oldPoints = acct.PointsBalance;
            var description = $"Allowance payout for {DateOnly.FromDateTime(DateTime.Today).ToShortDateString()}";

            acct.PayAllowanceToAccount();
            var transaction = acct.Transactions
                .SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));

            Assert.NotNull(transaction);
            Assert.Equal(ApprovalStatus.Pending, transaction.Status);
            Assert.Equal(allowance, transaction.Amount);
            Assert.NotEqual(oldPoints, acct.PointsBalance);
        }
        #endregion
    }

    public class AllowanceUtilityTests
    {
        #region IncOrDecPoint
        [Theory]
        [InlineData(PointOperation.Increment)]
        [InlineData(PointOperation.Decrement)]
        public void IncOrDecPoint_OperatesCorrectly(PointOperation operation)
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();
            var index = rng.Next(acct.Allowances.Count);

            var point = acct.Allowances[index];
            point.Points = rng.Next(10, 100);
            int oldCount = point.Points;

            point.IncOrDecPoint(operation);

            Assert.Equal(oldCount + (int)operation, point.Points);
        }


        [Fact]
        public void IncOrDecPoint_CantGoNegative()
        {
            var rng = Methods.GetRandomGenerator();
            Account acct = Methods.GenericAccount();
            var index = rng.Next(acct.Allowances.Count);

            var point = acct.Allowances[index];
            point.Points = 0;

            for (int i = 0; i != 5; i++)
            {
                point.IncOrDecPoint(PointOperation.Decrement);
            }

            Assert.Equal(0, point.Points);
        }
        #endregion
    }
}
