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
                var NextSunday = RightNow.Date.AddDays(((int)DayOfWeek.Sunday - (int)RightNow.DayOfWeek + 7) % 7);
                if (NextSunday <= RightNow)
                    NextSunday = NextSunday.AddDays(7);

                var Delay = NextSunday - RightNow;
                await Task.Delay(Delay, CancelToken);

                await IncrementBaseAllowances(CancelToken);
            }
        }

        private async Task IncrementBaseAllowances(CancellationToken CancelToken)
        {
            using var scope = _services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accounts = await accountService.GetAllAccountsAsync();
            foreach (var account in accounts)
            {
                await accountService.IncOrDecPointAsync(account.AccountID, "BaseAllowance", true);
            }
        }
    }
}