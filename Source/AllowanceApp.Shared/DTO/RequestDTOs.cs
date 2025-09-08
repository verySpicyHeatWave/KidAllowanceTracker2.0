using System.ComponentModel.DataAnnotations;

namespace AllowanceApp.Shared.DTO
{
    public record CreateAccountRequest
    {
        [Required]
        public string Name { get; set; }        
        public CreateAccountRequest(string name) { Name = name; }
    };

    public class TransactionRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Transaction amount must be greater than zero.")]
        public int Amount { get; set; }
        public string? Description { get; set; } = null;

        public TransactionRequest() { }

        public TransactionRequest(int amount, string? description = null)
        {
            Amount = amount;
            Description = description;
        }
    };

    public record PointUpdateRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Points must be greater than zero.")]
        public int Points { get; set; }
        public PointUpdateRequest(int points) { Points = points; }
    }

    public record TransactionStatusUpdateRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TransactionID must be greater than zero.")]
        public int TransactionID { get; set; }
    }
}