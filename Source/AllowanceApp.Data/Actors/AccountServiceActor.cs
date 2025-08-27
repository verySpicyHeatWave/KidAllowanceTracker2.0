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
            var account = new Account(name);
            await _context.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var accounts = await _context.Accounts
                .Include(a => a.Allowances)
                .Include(t => t.Transactions)
                .ToListAsync();
            if (accounts.Count == 0) { throw new DataNotFoundException("No accounts found in database."); }
            return accounts;
        }

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

        public async Task<AllowancePoint> SinglePointAdjustAsync(int id, string category, PointOperation operation)
        {
            var allowancePoint = await GetAllowancePointAsync(id, category);
            allowancePoint.IncOrDecPoint(operation);
            await _context.SaveChangesAsync();
            return allowancePoint;            
        }

        public async Task<AllowancePoint> UpdateAllowancePriceAsync(int id, string category, int amount)
        {
            var allowancePoint = await GetAllowancePointAsync(id, category);
            allowancePoint.Price = amount;
            await _context.SaveChangesAsync();
            return allowancePoint;
        }
        
        public async Task<Account> PayAllowanceAsync(int id)
        {
            var account = await GetAccountAsync(id);
            account.PayAllowanceToAccount();
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> ApplyTransactionAsync(int id, int amount, TransactionType action, string? description)
        {
            var account = await GetAccountAsync(id);
            account.ApplyTransaction(amount, action, description);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<string> DeleteAccountAsync(int id)
        {
            var account = await GetAccountAsync(id);
            _context.Remove(account);
            await _context.SaveChangesAsync();
            return account.Name;
        }
    }
}