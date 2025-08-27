using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Utilities
{
    public static class AllowanceUtility
    {
        public static int CalculateTotalAllowance(this Account account) =>
            Math.Max(0, account.Allowances.Sum(t => t.Total));

        public static void ResetPoints(this Account account) =>
            account.Allowances.ForEach(g => g.Points = 0);

        public static void IncOrDecPoint(this AllowancePoint point, bool Incrementing)
        {
            point.Points += Incrementing ? 1 : -1;
            if (point.Points < 0) { point.Points = 0; }
        }

        public static void SetPrice(this AllowancePoint point, bool Incrementing)
        {
        }
    }
}