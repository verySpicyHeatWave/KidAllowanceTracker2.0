using AllowanceApp.Avalonia.Models;
using AllowanceApp.Avalonia.Service;
using AllowanceApp.Avalonia.ViewModels.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AllowanceApp.Avalonia.ViewModels.Admin
{
    public partial class AdminViewModel : ViewModelBase
    {
        private List<Account> _accounts;
        private Dictionary<string, AccountOverviewViewModel> _overviewVms = [];
        private Dictionary<string, PointPricesViewModel> _priceVms = [];
        private Dictionary<string, TransactionApprovalsViewModel> _approvalsVms = [];

        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Greeting { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string SecretMessage { get; set; } = string.Empty;

        [ObservableProperty]
        public partial ObservableCollection<AccountComboBoxSelectionViewModel> Children { get; set; } = [];

        [ObservableProperty]
        public partial AccountComboBoxSelectionViewModel SelectedChild { get; set; }

        [ObservableProperty]
        public partial AccountOverviewViewModel AccountOverviewViewModel { get; set; }

        [ObservableProperty]
        public partial PointPricesViewModel PointPricesViewModel { get; set; }

        [ObservableProperty]
        public partial TransactionApprovalsViewModel TransactionApprovalsViewModel { get; set; }

        public AdminViewModel() : this(new ParentLicense()) { }

        public AdminViewModel(ParentLicense license)
        {
            Name = license.Name;
            Greeting = $"Hi, {Name}!";
            SecretMessage = license.SecretMessage;

            _accounts = AccountApiCaller.Instance.GetAllAccounts();

            foreach (var acct in _accounts)
            {
                Children.Add(new AccountComboBoxSelectionViewModel(acct));
                _overviewVms.Add(acct.Name, new AccountOverviewViewModel());
                _priceVms.Add(acct.Name, new PointPricesViewModel(acct));
                _approvalsVms.Add(acct.Name, new TransactionApprovalsViewModel());
            }
            SelectedChild = Children.First();
            AccountOverviewViewModel = _overviewVms[SelectedChild.Name];
            PointPricesViewModel = _priceVms[SelectedChild.Name];
            TransactionApprovalsViewModel = _approvalsVms[SelectedChild.Name];
        }

        partial void OnSelectedChildChanged(AccountComboBoxSelectionViewModel oldValue, AccountComboBoxSelectionViewModel newValue)
        {
            AccountOverviewViewModel = _overviewVms[newValue.Name];
            PointPricesViewModel = _priceVms[newValue.Name];
            TransactionApprovalsViewModel = _approvalsVms[newValue.Name];
        }
    }
}
