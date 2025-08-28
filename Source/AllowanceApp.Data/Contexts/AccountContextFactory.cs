using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AllowanceApp.Data.Contexts
{
    // I need this so that EFCore can use my context with options at DESIGN TIME.
    // I define and inject the context at runtime with the options that I want.
    // However, at design time, EF has no way to apply a config to my context.
    // This factory interface is found and used by EF to be able to use my context.

    // This is fuckery most supreme.
    [ExcludeFromCodeCoverage]
    public class AccountContextFactory : IDesignTimeDbContextFactory<AccountContext>
    {
        public AccountContext CreateDbContext(string[] args)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var DbPath = Path.Join(path, "accounts.db");

            var optionsBuilder = new DbContextOptionsBuilder<AccountContext>();
            optionsBuilder.UseSqlite($"Data Source={DbPath}");

            return new AccountContext(optionsBuilder.Options);
        }
    }
}