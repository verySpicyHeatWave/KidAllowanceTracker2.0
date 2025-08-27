using AllowanceApp.Core.Models;

namespace AllowanceApp.Core.Utilities
{
    public static class AllowanceUtility
    {
        public static void ResetPoints(this Account account) =>
            account.Allowances.ForEach(g => g.Points = 0);

        public static void IncOrDecPoint(this AllowancePoint point, PointOperation incOrDec)
        {
            point.Points += (int)incOrDec;
            if (point.Points < 0) { point.Points = 0; }
        }
    }
}