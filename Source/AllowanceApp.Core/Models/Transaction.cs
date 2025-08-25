namespace AllowanceApp.Core.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public int AccountID { get; set; }
        public double Amount { get; set; }
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
    }
}



