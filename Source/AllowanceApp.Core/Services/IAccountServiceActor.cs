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
        Task<AllowancePoint> UpdateAllowancePriceAsync(int id, string category, int amount);
        Task<Account> PayAllowanceAsync(int id);
        Task<Account> ApplyTransactionAsync(int id, int amount, TransactionType action, string? description);
        Task<string> DeleteAccountAsync(int id);
    }
}