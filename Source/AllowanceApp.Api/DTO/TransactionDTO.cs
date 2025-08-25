using AllowanceApp.Core.Models;

namespace AllowanceApp.Api.DTO
{
    public record TransactionDTO(int TransactionID, int AccountID, double Amount, DateOnly Date, string? Description)
    {
        public TransactionDTO(Transaction transaction) : this
        (
            transaction.TransactionID,
            transaction.AccountID,
            transaction.Amount,
            transaction.Date,
            transaction.Description
        ) { }
    }
}