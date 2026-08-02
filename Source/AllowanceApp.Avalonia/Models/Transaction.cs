using AllowanceApp.Shared.DTO;
using System;
using System.Text.Json.Serialization;

namespace AllowanceApp.Avalonia.Models
{
    public class Transaction
    {
        public int TransactionID { get; init; }
        public int AccountID { get; init; }
        public int Amount { get; init; }
        public int Status { get; init; } = 0;
        public DateOnly Date { get; init; } = DateOnly.FromDateTime(DateTime.Today);
        public string? Description { get; init; } = null;

        public Transaction(TransactionDTO dto) =>
        (TransactionID, AccountID, Amount, Status, Date, Description) =
        (
            dto.TransactionID,
            dto.AccountID,
            dto.Amount,
            (int)dto.Status,
            dto.Date,
            dto.Description
        );
    }
}
