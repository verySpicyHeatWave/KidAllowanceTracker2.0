using AllowanceApp.Avalonia.Utilities;
using AllowanceApp.Avalonia.ViewModels;
using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace AllowanceApp.Avalonia.Views
{
    public partial class AccountView : UserControl
    {
        public AccountView()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is not TabControl tabControl || !IsLoaded)
            {
                return;
            };

            if (tabControl.SelectedItem is TabItem selectedTab && selectedTab.Content is ContentControl contentControl && contentControl.Content is ViewModelBase vm)
            {
                var window = TopLevelLocator.GetTopLevelWindow(this);
                if (window != null && vm.Width > 0 && vm.Height > 0)
                {
                    window.Width = vm.Width;
                    window.Height = vm.Height;
                }
            }
        }
    }
}