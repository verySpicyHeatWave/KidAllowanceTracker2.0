using AllowanceApp.Avalonia.Models;
using AllowanceApp.Avalonia.Service;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.ViewModels.Accounts
{
    public class AccountViewModel : ViewModelBase
    {
        public string Name { get; init; } = "NoName";
        public string NameString => $"Hi, {Name}!";
        private int ID;
        public AllowanceViewModel AllowanceViewModel { get; set; }
        public TransactionsViewModel TransactionsViewModel { get; set; }
        public BankViewModel BankViewModel { get; set; }
        public AsyncRelayCommand RefreshAll { get; private set; }

        public AccountViewModel(Account acct)
        {
            Name = acct.Name;
            ID = acct.ID;
            AllowanceViewModel = new AllowanceViewModel(ID, acct.AllowancePoints);
            TransactionsViewModel = new TransactionsViewModel(acct.Transactions);
            BankViewModel = new BankViewModel(ID, acct.Balance);

            RefreshAll = new AsyncRelayCommand(OnRefreshAll);
        }

        public AccountViewModel() : this(new Account()) { }

        private async Task OnRefreshAll()
        {
            var account = await AccountApiCaller.Instance.GetAccountAsync(ID);
            if (account == null) return;

            AllowanceViewModel.Refresh(account);
            TransactionsViewModel.Refresh(account);
            BankViewModel.Refresh(account);

        }
    }
}