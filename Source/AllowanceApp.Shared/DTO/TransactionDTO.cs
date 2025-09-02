using System.Text.Json.Serialization;
using AllowanceApp.Core.Models;

namespace AllowanceApp.Shared.DTO
{
    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Declined
    }
    
    public record TransactionDTO
    {
        public int TransactionID { get; init; }
        public int AccountID { get; init; }
        public int Amount { get; init; }
        public DateOnly Date { get; init; } = DateOnly.FromDateTime(DateTime.Today);
        public string? Description { get; init; } = null;

        [JsonConstructor]
        public TransactionDTO() { }

        public TransactionDTO(Transaction transaction) =>
        (TransactionID, AccountID, Amount, Date, Description) =
        (
            transaction.TransactionID,
            transaction.AccountID,
            transaction.Amount,
            transaction.Date,
            transaction.Description
        );
    }
}