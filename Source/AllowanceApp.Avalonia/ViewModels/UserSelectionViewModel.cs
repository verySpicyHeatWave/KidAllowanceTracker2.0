
using AllowanceApp.Avalonia.Models;
using AllowanceApp.Avalonia.Utilities;
using AllowanceApp.Avalonia.ViewModels.Helpers;
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
        private ParentLicense? _parentLicense = null;

        public RelayCommand ButtonClick { get; set; }
        public ObservableCollection<AccountComboBoxSelectionViewModel> AccountList { get; set; }

        [ObservableProperty]
        public partial AccountComboBoxSelectionViewModel? SelectedAccount { get; set; }

        public event EventHandler<Account>? AccountConfirmed;

        public event EventHandler<ParentLicense>? ParentConfirmed;


        public UserSelectionViewModel() : this([]) { }

        public UserSelectionViewModel(List<Account> accountList)
        {
            accounts = accountList;
            AccountList = [.. accountList.Select(a => new AccountComboBoxSelectionViewModel(a))];
            if (CryptoUtils.TryValidateSignature(out _parentLicense))
            {
                AccountList.Add(new AccountComboBoxSelectionViewModel(_parentLicense));
            }
            SelectedAccount = null;
            ButtonClick = new RelayCommand(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var account = accounts.SingleOrDefault(a => a.ID == SelectedAccount?.Id);
            if (account == null)
            {
                if (_parentLicense is not null && SelectedAccount is not null && SelectedAccount.Id == _parentLicense.Id)
                {
                    ParentConfirmed?.Invoke(this, _parentLicense);
                }
            }
            else
            {
                AccountConfirmed?.Invoke(this, account);
            }
        }
    }
}
