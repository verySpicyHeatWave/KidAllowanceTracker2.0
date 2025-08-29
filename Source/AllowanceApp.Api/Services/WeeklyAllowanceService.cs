using AllowanceApp.Core.Models;
using AllowanceApp.Core.Services;

namespace AllowanceApp.Api.Services
{
    public class WeeklyAllowanceService(IServiceProvider services) : BackgroundService
    {
        private readonly IServiceProvider _services = services;

        protected override async Task ExecuteAsync(CancellationToken CancelToken)
        {
            while (!CancelToken.IsCancellationRequested)
            {
                var RightNow = DateTime.Now;
                var NextSunday = CalculateNextSunday(RightNow);

                var Delay = NextSunday - RightNow;
                await Task.Delay(Delay, CancelToken);

                await IncrementBaseAllowances();
            }
        }

        internal static DateTime CalculateNextSunday(DateTime now)
        {
            var next = now.Date.AddDays(((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7);
            return next <= now ? next.AddDays(7) : next;
        }

        private async Task IncrementBaseAllowances()
        {
            using var scope = _services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accounts = await accountService.GetAllAccountsAsync();
            foreach (var account in accounts)
            {
                await accountService.SinglePointAdjustAsync(account.AccountID, "BaseAllowance", PointOperation.Increment);
            }
        }
    }
}