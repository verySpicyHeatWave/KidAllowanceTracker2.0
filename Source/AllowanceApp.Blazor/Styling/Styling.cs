using AllowanceApp.Blazor.Models;

namespace AllowanceApp.Blazor.Styling
{
    public static class Styling
    {

        public static Dictionary<ApprovalStatus, string> ApprovalTextColorCSS = new()
        {
            {ApprovalStatus.Approved, "text-green-600 dark:text-green-400"},
            {ApprovalStatus.Declined, "text-red-600 dark:text-red-400"},
            {ApprovalStatus.Pending, "text-gray-400 dark:text-gray-600 italic"},
        };
    }
}