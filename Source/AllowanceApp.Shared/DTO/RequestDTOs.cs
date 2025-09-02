namespace AllowanceApp.Shared.DTO
{
    public record CreateAccountRequest(string Name);

    public record TransactionRequest
    {        
        public int Amount { get; set; }
        public string? Description { get; set; } = null;
    };

    public record PointUpdateRequest(int Points);
}