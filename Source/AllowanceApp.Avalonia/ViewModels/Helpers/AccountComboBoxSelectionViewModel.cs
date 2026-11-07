using AllowanceApp.Avalonia.Models;

namespace AllowanceApp.Avalonia.ViewModels.Helpers
{
    public class AccountComboBoxSelectionViewModel(Account account) : ViewModelBase
    {
        public int Id { get; set; } = account.ID;
        public string Name { get; set; } = account.Name;

        public AccountComboBoxSelectionViewModel(ParentLicense parentLicense) : this(new Account())
        {
            Id = parentLicense.Id;
            Name = parentLicense.Name;
        }
    }
}
