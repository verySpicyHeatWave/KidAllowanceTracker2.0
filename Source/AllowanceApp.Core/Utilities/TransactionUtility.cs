using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Utilities
{
    public static class TransactionUtility
    {
        public static int CalculateTotalAllowance(Account account) =>
            account.Allowances.Sum(t => t.Total);

        public static void PayAllowanceToAccount(Account account)
        {
            int Total = CalculateTotalAllowance(account);
            ResetPoints(account);
            string Description = $"Allowance payout for {DateOnly.FromDateTime(DateTime.Today).ToShortDateString()}";
            ApplyTransaction(account, Total, false, Description);
        }

        public static void ApplyTransaction(Account account, int amount, bool isWithdrawal, string? description)
        {
            if (account.Balance == 0 && isWithdrawal) { return; }
            var xferAmount = isWithdrawal ? -amount : amount;
            var oldBalance = account.Balance;
            account.Balance += xferAmount;
            if (account.Balance < 0)
            {
                account.Balance = 0;
                amount = oldBalance;
            }
            Transaction transaction = new()
            {
                AccountID = account.AccountID,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Amount = amount,
                Description = description
            };
            account.Transactions.Add(transaction);
        }

        private static void ResetPoints(Account account)
        {
            account.Allowances.ForEach(g => g.Points = 0);
        }
    }
}