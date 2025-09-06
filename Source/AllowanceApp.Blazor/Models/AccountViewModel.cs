using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Blazor.Models
{
    public class AccountViewModel
    {
        public string Name { get; init; } = string.Empty;
        public int ID { get; init; }
        public int Balance { get; set; }
        public AllowancesViewModel Allowances { get; set; }
        public TransactionsViewModel Transactions { get; set; }
        private int _allowanceTotal => Math.Max(0,Allowances.PointList.Sum(a => a.Total));
        public string AllowanceDisplay => (_allowanceTotal/100.0).ToString("#0.00");
        public string BalanceDisplay => (Balance/100.0).ToString("#0.00");

        public AccountViewModel(AccountDTO dto)
        {
            Name = dto.Name;
            ID = dto.ID;
            Balance = dto.Balance;
            Allowances = new AllowancesViewModel(dto.Allowances);
            Transactions = new TransactionsViewModel(dto.Transactions);
        }
    }
}