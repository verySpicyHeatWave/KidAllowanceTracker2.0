using AllowanceApp.Avalonia.Models;
using System.Collections.Generic;

namespace AllowanceApp.Avalonia.ViewModels
{
    public class TransactionsViewModel : ViewModelBase
    {
        public List<Transaction> TransactionList { get; set; } = [];

        public TransactionsViewModel() : this([]) { }

        public TransactionsViewModel(List<Transaction> transactions)
        {
            TransactionList = transactions;
        }
    }
}