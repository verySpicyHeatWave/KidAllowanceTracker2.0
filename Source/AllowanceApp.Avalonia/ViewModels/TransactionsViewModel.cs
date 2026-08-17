using AllowanceApp.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AllowanceApp.Avalonia.ViewModels
{
    public enum CategoryFilterEnum
    {
        Pending,
        Approved,
        Declined,
        All
    }

    public enum HistoryFilterEnum
    {
        All,
        Days90,
        Days30,
        Days7
    }

    public class TransactionsViewModel : ViewModelBase
    {
        public List<Transaction> TransactionList { get; set; } = [];
        public ObservableCollection<Transaction> TransactionDisplayList { get; set; } = [];
        public ObservableCollection<ComboBoxItemViewModel<CategoryFilterEnum>> CategoryFilterList { get; set; } =
        [
            new ComboBoxItemViewModel<CategoryFilterEnum>("All", CategoryFilterEnum.All),
            new ComboBoxItemViewModel<CategoryFilterEnum>("Approved", CategoryFilterEnum.Approved),
            new ComboBoxItemViewModel<CategoryFilterEnum>("Declined", CategoryFilterEnum.Declined),
            new ComboBoxItemViewModel<CategoryFilterEnum>("Pending", CategoryFilterEnum.Pending)
        ];
        public ComboBoxItemViewModel<CategoryFilterEnum> CategoryFilter { get; set; }
        public ObservableCollection<ComboBoxItemViewModel<HistoryFilterEnum>> HistoryFilterList { get; set; } =
        [
            new ComboBoxItemViewModel<HistoryFilterEnum>("All", HistoryFilterEnum.All),
            new ComboBoxItemViewModel<HistoryFilterEnum>("7 Days", HistoryFilterEnum.Days7),
            new ComboBoxItemViewModel<HistoryFilterEnum>("30 Days", HistoryFilterEnum.Days30),
            new ComboBoxItemViewModel<HistoryFilterEnum>("90 Days", HistoryFilterEnum.Days90)
        ];
        public ComboBoxItemViewModel<HistoryFilterEnum> HistoryFilter { get; set; }

        public TransactionsViewModel() : this([]) { }

        public TransactionsViewModel(List<Transaction> transactions)
        {
            TransactionList = transactions;
            Width = 1000;
            Height = 650;
            CategoryFilter = CategoryFilterList.First();
            HistoryFilter = HistoryFilterList.First();
            UpdateTransactionDisplayList();
        }

        public void UpdateTransactionDisplayList()
        {
            TransactionDisplayList.Clear();
            Transaction[] array = new Transaction[TransactionList.Count];
            TransactionList.CopyTo(array);

            List<Transaction> list = [.. array
                .Where(ApplyCategoryFilter)
                .Where(ApplyHistoryFilter)
            ];

            foreach (var t in list) TransactionDisplayList.Add(t);
        }

        private bool ApplyCategoryFilter(Transaction t)
        {
            if (CategoryFilter.Value == CategoryFilterEnum.All)
                return true;
            return (CategoryFilterEnum)t.Status == CategoryFilter.Value;
        }

        private bool ApplyHistoryFilter(Transaction t)
        {
            bool value = HistoryFilter.Value switch
            {
                HistoryFilterEnum.Days7 => t.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
                HistoryFilterEnum.Days30 => t.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
                HistoryFilterEnum.Days90 => t.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-90)),
                _ => true
            };
            return value;
        }
    }
}