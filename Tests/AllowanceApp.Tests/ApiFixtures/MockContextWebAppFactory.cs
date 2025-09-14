using AllowanceApp.Data.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AllowanceApp.Tests.ApiFixtures
{
    public class MockContextWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;
        public MockContextWebAppFactory()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AccountContext>));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AccountContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AccountContext>();
                db.Database.EnsureCreated();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        public void ResetDatabase()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AccountContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        public void SeedDatabase(Action<AccountContext> seedAction)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AccountContext>();
            seedAction(db);
            db.SaveChanges();
        }
    }
}
