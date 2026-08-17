using AllowanceApp.Avalonia.Models;
using System;
using System.Linq;

namespace AllowanceApp.Avalonia.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        public string Name { get; init; } = "NoName";
        public string NameString => $"{Name}'s Allowance";
        public int ID { get; init; }
        public int Balance { get; set; }
        public AllowanceViewModel AllowanceViewModel { get; set; }
        public TransactionsViewModel TransactionsViewModel { get; set; }
        public BankViewModel BankViewModel { get; set; }
        private int _allowanceTotal => Math.Max(0, AllowanceViewModel.PointList.Sum(a => a.Total));
        public string AllowanceDisplay => (_allowanceTotal / 100.0).ToString("#0.00");
        public string BalanceDisplay => (Balance / 100.0).ToString("#0.00");

        public AccountViewModel(Account acct)
        {
            Name = acct.Name;
            ID = acct.ID;
            Balance = acct.Balance;
            AllowanceViewModel = new AllowanceViewModel(ID, acct.AllowancePoints);
            TransactionsViewModel = new TransactionsViewModel(acct.Transactions);
            BankViewModel = new BankViewModel();
            Width = 400;
            Height = 650;
        }

        public AccountViewModel() : this(new Account()) { }
    }
}