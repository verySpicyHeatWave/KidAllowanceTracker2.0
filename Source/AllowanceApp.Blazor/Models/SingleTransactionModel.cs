using System.Data;
using AllowanceApp.Core.Models;
using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Blazor.Models
{
    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Declined
    }
    
    public class SingleTransactionModel(TransactionDTO dto)
    {
        public int TransactionID { get; init; } = dto.TransactionID;
        public int AccountID { get; init; } = dto.AccountID;
        public ApprovalStatus Status { get; set; } = (ApprovalStatus)dto.Status;
        public int Amount { get; init; } = dto.Amount;
        public DateOnly Date { get; init; } = dto.Date;
        public string Description { get; init; } = dto.Description ?? string.Empty;
    }
}