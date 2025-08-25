using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Services
{
    public class AccountService(IAccountServiceActor repository)
    {
        private readonly IAccountServiceActor _repository = repository;

        public Task<Account> AddAccountAsync(string name) =>
            _repository.AddAccountAsync(name);

        public Task<List<Account>> GetAllAccountsAsync() =>
            _repository.GetAllAccountsAsync();

        public Task<Account> GetAccountAsync(int id) =>
            _repository.GetAccountAsync(id);

        public Task<AllowancePoint> GetAllowancePointAsync(int id, string category) =>
        _repository.GetAllowancePointAsync(id, category);

        public Task<AllowancePoint> IncOrDecPointAsync(int id, string category, bool incrementing) =>
            _repository.IncOrDecPointAsync(id, category, incrementing);

        public Task<AllowancePoint> UpdateAllowancePriceAsync(int id, string category, double amount) =>
            _repository.UpdateAllowancePriceAsync(id, category, amount);

        public Task<Account> PayAllowanceAsync(int id) =>
            _repository.PayAllowanceAsync(id);

        public Task<Account> ApplyTransactionAsync(int id, double amount, bool isWithdrawal, string? description) =>
            _repository.ApplyTransactionAsync(id, amount, isWithdrawal, description);

        public Task<string> DeleteAccountAsync(int id) =>
            _repository.DeleteAccountAsync(id);
    }
}