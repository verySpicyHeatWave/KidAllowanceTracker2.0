using AllowanceApp.Avalonia.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AllowanceApp.Avalonia.ViewModels
{
    public class AccountComboBoxSelectionViewModel(Account account) : ViewModelBase
    {
        public int Id { get; set; } = account.ID;
        public string Name { get; set; } = account.Name;
    }
}
