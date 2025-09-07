using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Services
{
    public interface IAccountServiceActor
    {
        Task<Account> AddAccountAsync(string name);
        Task<Account> GetAccountAsync(int id);
        Task<List<Account>> GetAllAccountsAsync();
        Task<AllowancePoint> GetAllowancePointAsync(int id, string category);
        Task<AllowancePoint> SinglePointAdjustAsync(int id, string category, PointOperation operation);
        Task<AllowancePoint> SetPointAsync(int id, string category, int value);
        Task<AllowancePoint> UpdateAllowancePriceAsync(int id, string category, int amount);
        Task<Account> PayAllowanceAsync(int id);
        Task<Account> RequestTransactionAsync(int id, int amount, TransactionType action, string? description);
        Task<Account> ApproveTransactionAsync(int id, int transaction_id);
        Task<Account> DeclineTransactionAsync(int id, int transaction_id);
        Task<string> DeleteAccountAsync(int id);
    }
}