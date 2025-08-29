using AllowanceApp.Api.Services;
using AllowanceApp.Data.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AllowanceApp.Tests.ApiFixtures
{
    public class BasePointServiceWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;
        public BasePointServiceWebAppFactory()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                var real_context = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AccountContext>));
                if (real_context is not null)
                {
                    services.Remove(real_context);
                }
                var real_service = services.SingleOrDefault(
                    d => d.ServiceType == typeof(WeeklyAllowanceService));
                if (real_service is not null)
                {
                    services.Remove(real_service);
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

        // public void ResetDatabase()
        // {
        //     using var scope = Services.CreateScope();
        //     var db = scope.ServiceProvider.GetRequiredService<AccountContext>();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();
        // }

        // public void SeedDatabase(Action<AccountContext> seedAction)
        // {
        //     using var scope = Services.CreateScope();
        //     var db = scope.ServiceProvider.GetRequiredService<AccountContext>();
        //     seedAction(db);
        //     db.SaveChanges();
        // }
    }
}
