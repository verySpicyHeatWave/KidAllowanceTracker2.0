
using AllowanceApp.Avalonia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AllowanceApp.Avalonia.ViewModels
{
    public partial class UserSelectionViewModel : ViewModelBase
    {
        private List<Account> accounts;

        public RelayCommand ButtonClick { get; set; }
        public ObservableCollection<AccountComboBoxSelectionViewModel> AccountList { get; set; }

        [ObservableProperty]
        public partial AccountComboBoxSelectionViewModel? SelectedAccount { get; set; }

        public event EventHandler<Account>? AccountConfirmed;

        public UserSelectionViewModel(List<Account> accountList)
        {
            accounts = accountList;
            AccountList = [.. accountList.Select(a => new AccountComboBoxSelectionViewModel(a))];
            SelectedAccount = null;
            ButtonClick = new RelayCommand(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var account = accounts.SingleOrDefault(a => a.ID == SelectedAccount?.Id);
            if (account == null) { return; }
            AccountConfirmed?.Invoke(this, account);
        }
    }
}
