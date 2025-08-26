using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Services
{
    public class AccountService(IAccountServiceActor actor)
    {
        private readonly IAccountServiceActor _actor = actor;

        public async Task<Account> AddAccountAsync(string name) =>
            await _actor.AddAccountAsync(name);

        public async Task<List<Account>> GetAllAccountsAsync() =>
            await _actor.GetAllAccountsAsync();

        public async Task<Account> GetAccountAsync(int id) =>
            await _actor.GetAccountAsync(id);

        public async Task<AllowancePoint> GetAllowancePointAsync(int id, string category) =>
            await _actor.GetAllowancePointAsync(id, category);

        public async Task<AllowancePoint> IncOrDecPointAsync(int id, string category, bool incrementing) =>
            await _actor.IncOrDecPointAsync(id, category, incrementing);

        public async Task<AllowancePoint> UpdateAllowancePriceAsync(int id, string category, double amount) =>
            await _actor.UpdateAllowancePriceAsync(id, category, amount);

        public async Task<Account> PayAllowanceAsync(int id) =>
            await _actor.PayAllowanceAsync(id);

        public async Task<Account> ApplyTransactionAsync(int id, double amount, bool isWithdrawal, string? description) =>
            await _actor.ApplyTransactionAsync(id, amount, isWithdrawal, description);

        public async Task<string> DeleteAccountAsync(int id) =>
            await _actor.DeleteAccountAsync(id);
    }
}