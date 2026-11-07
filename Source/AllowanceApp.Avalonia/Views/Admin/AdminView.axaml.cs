using AllowanceApp.Avalonia.Utilities;
using AllowanceApp.Avalonia.ViewModels;
using Avalonia.Controls;

namespace AllowanceApp.Avalonia.Views.Admin
{
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is not TabControl tabControl || !IsLoaded)
            {
                return;
            }

            if (tabControl.SelectedItem is TabItem selectedTab && selectedTab.Content is ContentControl contentControl && contentControl.Content is ViewModelBase vm)
            {
                var window = TopLevelLocator.GetTopLevelWindow(this);
                if (window != null && vm.Width > 0 && vm.Height > 0)
                {
                    window.Width = vm.Width;
                    window.Height = vm.Height;
                    tabControl.Width = vm.Width;
                    tabControl.Height = vm.Height;
                }
            }
        }
    }
}