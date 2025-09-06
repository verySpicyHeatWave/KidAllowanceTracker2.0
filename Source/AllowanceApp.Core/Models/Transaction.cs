namespace AllowanceApp.Core.Models
{
    public enum TransactionType
    {
        Withdraw = -1,
        Deposit = 1
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Declined
    }

    public class Transaction
    {
        public int TransactionID { get; set; }
        public int AccountID { get; set; }
        public int Amount { get; set; }
        public ApprovalStatus Status { get; set; }
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
    }
}



