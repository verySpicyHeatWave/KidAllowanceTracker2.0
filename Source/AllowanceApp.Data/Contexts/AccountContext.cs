using AllowanceApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AllowanceApp.Data.Contexts
{
    public class AccountContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AllowancePoint> AllowancePoints { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public string DbPath { get; }
        public AccountContext(DbContextOptions<AccountContext> options)
            : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "accounts.db");
        }

        public AccountContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "accounts.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasKey(e => e.AccountID);
            modelBuilder.Entity<Account>()
                .Property(e => e.AccountID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Account>()
                .HasIndex(e => e.Name)
                .IsUnique();

            modelBuilder.Entity<AllowancePoint>()
                .HasKey(a => new {a.AccountID, a.Category});
            modelBuilder.Entity<AllowancePoint>()
                .HasOne<Account>()
                .WithMany(a => a.Allowances)
                .HasForeignKey(p => p.AccountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionID);
            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Transaction>()
                .HasOne<Account>()
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
