using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Utilities
{
    public static class TransactionUtility
    {
        public static double CalculateTotalAllowance(Account account) =>
            account.Allowances.Sum(t => t.Total);

        public static void PayAllowanceToAccount(Account account)
        {
            double Total = CalculateTotalAllowance(account);
            ResetPoints(account);
            string Description = $"Allowance payout for {DateOnly.FromDateTime(DateTime.Today).ToShortDateString()}";
            ApplyTransaction(account, Total, false, Description);
        }

        public static void ApplyTransaction(Account account, double amount, bool isWithdrawal, string? description)
        {
            if (isWithdrawal) { amount *= -1; }
            account.Balance += amount;
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