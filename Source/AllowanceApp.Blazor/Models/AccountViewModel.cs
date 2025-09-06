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