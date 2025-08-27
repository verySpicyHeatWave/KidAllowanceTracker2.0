using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Utilities
{
    public static class TransactionUtility
    {
        public static void PayAllowanceToAccount(this Account account)
        {
            const bool DEPOSIT = false;
            int Total = account.CalculateTotalAllowance();
            account.ResetPoints();
            string Description = $"Allowance payout for {DateOnly.FromDateTime(DateTime.Today).ToShortDateString()}";
            ApplyTransaction(account, Total, DEPOSIT, Description);
        }

        public static void ApplyTransaction(this Account account, int amount, bool isWithdrawal, string? description)
        {
            if (amount <= 0) { return; }
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
    }
}