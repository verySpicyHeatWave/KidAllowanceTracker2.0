using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Utilities
{
    public static class TransactionUtility
    {
        public static void PayAllowanceToAccount(this Account account)
        {
            int Total = account.AllowanceBalance;
            account.ResetPoints();
            string Description = $"Allowance payout for {DateOnly.FromDateTime(DateTime.Today).ToShortDateString()}";
            ApplyTransaction(account, Total, TransactionType.Deposit, Description);
        }

        public static void ApplyTransaction(this Account account, int amount, TransactionType action, string? description)
        {
            if (InvalidWithdrawal(account, action, amount)) { return; }
            var xferAmount = amount * (int)action;
            var oldBalance = account.Balance;
            account.Balance += xferAmount;
            amount = PreventOverdraft(account, amount, oldBalance);

            Transaction transaction = new()
            {
                AccountID = account.AccountID,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Amount = amount,
                Description = description
            };
            account.Transactions.Add(transaction);
        }

        private static int PreventOverdraft(Account account, int amount, int oldbalance)
        {
            var resp = account.Balance < 0 ? oldbalance : amount;
            if (resp == oldbalance) { account.Balance = 0; }
            return resp;
        }

        private static bool InvalidWithdrawal(Account account, TransactionType action, int amount) =>
            (account.Balance == 0 && action == TransactionType.Withdraw) || (amount <= 0);
    }
}