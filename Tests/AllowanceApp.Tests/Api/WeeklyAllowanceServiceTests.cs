using System.Net;
using System.Net.Http.Json;
using AllowanceApp.Core.Models;
using AllowanceApp.Shared.DTO;
using AllowanceApp.Tests.Common;
using AllowanceApp.Tests.ApiFixtures;
using AllowanceApp.Api.Services;

namespace AllowanceApp.Tests.Api
{
    public class WeeklyAllowanceServiceTests(MockContextWebAppFactory factory) : IClassFixture<MockContextWebAppFactory>
    {
        private readonly MockContextWebAppFactory _factory = factory;
        
        [Fact]
        public static void CalculateNextSunday_WorksForEveryDay()
        {
            var expected_val = (DayOfWeek.Sunday - DateTime.Now.DayOfWeek + 7) % 7;
            var NextDay = WeeklyAllowanceService.CalculateNextSunday(DateTime.Now);
            Assert.Equal(expected_val, (NextDay - DateTime.Today).Days);
        }
    }
}