using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Utilities
{
    public static class TransactionUtility
    {
        public static void PayAllowanceToAccount(this Account account)
        {
            int Total = account.CalculateTotalAllowance();
            account.ResetPoints();
            string Description = $"Allowance payout for {DateOnly.FromDateTime(DateTime.Today).ToShortDateString()}";
            ApplyTransaction(account, Total, TransactionType.Deposit, Description);
        }

        public static void ApplyTransaction(this Account account, int amount, TransactionType action, string? description)
        {
            if (amount <= 0) { return; }
            if (account.Balance == 0 && action == TransactionType.Withdraw) { return; }
            var xferAmount = amount * (int)action;
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
    }
}