using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AllowanceApp.Data.Contexts
{
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