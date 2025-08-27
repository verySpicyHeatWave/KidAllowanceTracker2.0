using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Utilities
{
    public static class AllowanceUtility
    {
        public static int CalculateTotalAllowance(this Account account) =>
            Math.Max(0, account.Allowances.Sum(t => t.Total));

        public static void ResetPoints(this Account account) =>
            account.Allowances.ForEach(g => g.Points = 0);

        public static int GetPointsTotal(this Account account) =>
            account.Allowances.Sum(t => t.Points);

        public static void IncOrDecPoint(this AllowancePoint point, PointOperation incOrDec)
        {
            point.Points *= (int)incOrDec;
            if (point.Points < 0) { point.Points = 0; }
        }
    }
}