using Avalonia;
using Avalonia.Controls;

namespace AllowanceApp.Avalonia.Utilities
{
    internal class TopLevelLocator
    {
        public static Window? GetTopLevelWindow(Visual visual)
        {
            var topLevel = TopLevel.GetTopLevel(visual);
            if (topLevel is Window window)
            {
                return window;
            }
            return null;
        }
    }
}
