using AllowanceApp.Shared.DTO;
using System.Collections.Generic;
using System.Linq;

namespace AllowanceApp.Avalonia.Models
{
    public class Account
    {
        public string Name { get; init; } = "NoName";
        public int ID { get; init; }
        public int Balance { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<AllowancePoint> AllowancePoints { get; set; }
        public Account(AccountDTO dto)
        {
            Name = dto.Name;
            ID = dto.ID;
            Balance = dto.Balance;
            Transactions = dto.Transactions
                .Select(t => new Transaction(t))
                .OrderByDescending(t => t.TransactionID)
                .ToList();
            AllowancePoints = dto.Allowances
                .Select(a => new AllowancePoint(a))
                .ToList();
        }

        public Account()
        {
            ID = -1;
            Balance = -1;
            Transactions = [];
            AllowancePoints = [];
        }
    }
}
