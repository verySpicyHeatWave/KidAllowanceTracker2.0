using AllowanceApp.Core.Models;
using AllowanceApp.Core.Services;
using AllowanceApp.Core.Utilities;
using AllowanceApp.Data.Contexts;
using AllowanceApp.Data.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AllowanceApp.Data.Actors
{
    public class AccountServiceActor(AccountContext context) : IAccountServiceActor
    {
        private readonly AccountContext _context = context;

        public async Task<Account> AddAccountAsync(string name)
        {
            var account = new Account(name) { Name = name };
            await _context.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<List<Account>> GetAllAccountsAsync() =>
            await _context.Accounts
                .Include(a => a.Allowances)
                .Include(t => t.Transactions)
                .ToListAsync()
                ?? throw new DataNotFoundException("No accounts found in database.");

        public async Task<Account> GetAccountAsync(int id) =>
            await _context.Accounts
                .Include(a => a.Allowances)
                .Include(t => t.Transactions)
                .FirstOrDefaultAsync(a => a.AccountID == id)
                ?? throw new DataNotFoundException($"No account found with ID number {id}");

        public async Task<AllowancePoint> GetAllowancePointAsync(int id, string category)
        {
            var account = await GetAccountAsync(id);
            var name = account.Name;

            var allowancePoint = account.Allowances
                .SingleOrDefault(pt => string.Compare(
                    pt.Category,
                    category,
                    StringComparison.OrdinalIgnoreCase
                ) == 0) ?? throw new DataNotFoundException($"Could not find allowance type {category} belonging to {name}");
            return allowancePoint;
        }

        public async Task<AllowancePoint> IncOrDecPointAsync(int id, string category, bool incrementing)
        {
            var allowancePoint = await GetAllowancePointAsync(id, category);
            allowancePoint.Points += incrementing ? 1 : -1;
            if (allowancePoint.Points < 0) { allowancePoint.Points = 0; }
            await _context.SaveChangesAsync();
            return allowancePoint;            
        }

        public async Task<AllowancePoint> UpdateAllowancePriceAsync(int id, string category, double amount)
        {
            var allowancePoint = await GetAllowancePointAsync(id, category);
            allowancePoint.Price = amount;
            await _context.SaveChangesAsync();
            return allowancePoint;
        }
        
        public async Task<Account> PayAllowanceAsync(int id)
        {
            var account = await GetAccountAsync(id) ?? throw new DataNotFoundException($"No account found with ID number {id}");
            if (TransactionUtility.CalculateTotalAllowance(account) == 0) { return account; }
            TransactionUtility.PayAllowanceToAccount(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> ApplyTransactionAsync(int id, double amount, bool isWithdrawal, string? description)
        {
            var account = await GetAccountAsync(id) ?? throw new DataNotFoundException($"No account found with ID number {id}");
            if (amount <= 0) { return account; }
            TransactionUtility.ApplyTransaction(account, amount, isWithdrawal, description);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<string> DeleteAccountAsync(int id)
        {
            var account = await GetAccountAsync(id) ?? throw new DataNotFoundException($"No account found with ID number {id}");
            _context.Remove(account);
            await _context.SaveChangesAsync();
            return account.Name;
        }
    }
}