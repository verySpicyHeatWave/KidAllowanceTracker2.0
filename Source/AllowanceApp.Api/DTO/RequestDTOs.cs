public record CreateAccountRequest(string Name);

public record TransactionRequest(int Amount, string? Description);