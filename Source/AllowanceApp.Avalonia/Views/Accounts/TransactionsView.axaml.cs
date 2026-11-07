using AllowanceApp.Avalonia.ViewModels.Accounts;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AllowanceApp.Avalonia.Views.Accounts
{
    public partial class TransactionsView : UserControl
    {
        public TransactionsView()
        {
            InitializeComponent();
        }

        private void OnFilterChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TransactionsViewModel vm) 
                vm.UpdateTransactionDisplayList();
        }
    }
}