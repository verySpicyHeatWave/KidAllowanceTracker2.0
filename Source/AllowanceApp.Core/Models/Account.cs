using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic;

namespace AllowanceApp.Core.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public required string Name { get; set; }
        public int Balance { get; set; } = 0;
        public int PointsBalance => Allowances.Sum(t => t.Points);
        public int AllowanceBalance => Math.Max(0, Allowances.Sum(t => t.Total));
        public List<AllowancePoint> Allowances { get; set; } = [];
        public List<Transaction> Transactions { get; set; } = [];

        protected Account() { }

        [SetsRequiredMembers] 
        public Account(string name)
        {
            Name = name;
            PopulateDefaultAllowancePoints();
        }

        public void ResetPoints() => Allowances.ForEach(g => g.Points = 0);


        public void PayAllowanceToAccount()
        {
            int Total = AllowanceBalance;
            ResetPoints();
            string Description = $"Allowance payout for {DateOnly.FromDateTime(DateTime.Today).ToShortDateString()}";
            RequestTransaction(Total, TransactionType.Deposit, Description);
        }

        public void RequestTransaction(int amount, TransactionType action, string? description)
        {
            if (InvalidWithdrawal(action, amount)) { return; }

            Transaction transaction = new()
            {
                AccountID = AccountID,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Amount = amount * (int)action,
                Description = description,
                Status = ApprovalStatus.Pending
            };
            Transactions.Add(transaction);
        }

        public void ApproveTransaction(int transactionID)
        {
            var transaction = Transactions.First(t => t.TransactionID == transactionID);
            if (transaction is null) { return; }
            transaction.Status = ApprovalStatus.Approved;
            Console.WriteLine($"================== Nah bruh! ==================");

            var oldBalance = Balance;
            Balance += transaction.Amount;

            if (transaction.Amount < 0)
            {
                transaction.Amount = PreventOverdraft(transaction.Amount, oldBalance);
            }

        }

        public void DeclineTransaction(int transactionID)
        {
            var transaction = Transactions.First(t => t.TransactionID == transactionID);
            if (transaction is null) { return; }
            transaction.Status = ApprovalStatus.Declined;
        }

        private int PreventOverdraft(int amount, int oldBalance)
        {
            var resp = Balance < 0 ? -oldBalance : amount;
            Balance = Math.Max(0, Balance);
            return resp;
        }

        private bool InvalidWithdrawal(TransactionType action, int amount) =>
            (Balance == 0 && action == TransactionType.Withdraw) || (amount <= 0);

        private void PopulateDefaultAllowancePoints()
        {
            Allowances =
            [
                new AllowancePoint("BaseAllowance", 500) { Points = 1 },
                new AllowancePoint("GoodBehavior", 100),
                new AllowancePoint("Homework", 100),
                new AllowancePoint("Chores", 100),
                new AllowancePoint("BadBehavior", -50),
                new AllowancePoint("GradeA", 500),
                new AllowancePoint("GradeB", 300),
                new AllowancePoint("GradeC", 100),
                new AllowancePoint("GradeD", 0),
                new AllowancePoint("GradeF", -50)
            ];
        }
    }
}
