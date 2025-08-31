namespace AllowanceApp.Shared.DTO
{
    public record CreateAccountRequest(string Name);

    public record TransactionRequest(int Amount, string? Description);

    public record PointUpdateRequest(int Points);
}