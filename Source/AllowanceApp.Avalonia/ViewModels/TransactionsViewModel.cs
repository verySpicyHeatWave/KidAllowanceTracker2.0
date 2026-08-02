using AllowanceApp.Avalonia.Models;
using AllowanceApp.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Text;

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