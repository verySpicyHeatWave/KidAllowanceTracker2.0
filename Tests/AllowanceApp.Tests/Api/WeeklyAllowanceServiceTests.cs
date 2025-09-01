using System.Net;
using System.Net.Http.Json;
using AllowanceApp.Core.Models;
using AllowanceApp.Shared.DTO;
using AllowanceApp.Tests.Common;
using AllowanceApp.Tests.ApiFixtures;
using AllowanceApp.Api.Services;
using AllowanceApp.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AllowanceApp.Tests.Api
{
    public class WeeklyAllowanceServiceTests
    {
        [Fact]
        public static void CalculateNextSunday_WorksForEveryDay()
        {
            var expected_val = (DayOfWeek.Sunday - DateTime.Now.DayOfWeek + 7) % 7;
            if (expected_val == 0) {expected_val = 7;} // Because it's Sunday and so NEXT Sunday is seven days from now
            var NextDay = WeeklyAllowanceService.CalculateNextSunday(DateTime.Now);
            Assert.Equal(expected_val, (NextDay - DateTime.Today).Days);
        }

        [Fact]
        public async Task IncrementBaseAllowances_IncrementsEachAccount()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new("Adam") {AccountID = 1},
                new("Beth") {AccountID = 2},
                new("Carl") {AccountID = 3},
                new("Dave") {AccountID = 4},
                new("Erin") {AccountID = 5 }
            };

            var mockActor = new Mock<IAccountServiceActor>();
            mockActor
                .Setup(s => s.GetAllAccountsAsync())
                .ReturnsAsync(accounts);

            mockActor
                .Setup(s => s.SinglePointAdjustAsync(It.IsAny<int>(), "BaseAllowance", PointOperation.Increment))
                .ReturnsAsync((int id, string category, PointOperation op) =>
                    new AllowancePoint(category, 1) { AccountID = id });

            var provider = new ServiceCollection()
                .AddScoped(_ => mockActor.Object)
                .BuildServiceProvider();

            var service = new WeeklyAllowanceService(provider);
            await service.IncrementBaseAllowances();

            mockActor.Verify(s => s.GetAllAccountsAsync(), Times.Once);

            for (int i = 1; i != 6; i++)
            {
                mockActor.Verify(
                    s => s.SinglePointAdjustAsync(i, "BaseAllowance", PointOperation.Increment), Times.Once);
            }
        }
    }
}