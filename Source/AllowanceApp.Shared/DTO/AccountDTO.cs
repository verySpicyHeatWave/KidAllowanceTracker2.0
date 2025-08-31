using AllowanceApp.Core.Models;
using AllowanceApp.Shared.Utilities;
using System.Text.Json.Serialization;

namespace AllowanceApp.Shared.DTO
{
    public record AccountDTO
    {
        public string Name { get; init; } = string.Empty;
        public int ID { get; init; }
        public int Balance { get; set; }
        public int AllowanceTotal { get; set; }
        public List<AllowancePointDTO> Allowances { get; set; } = [];
        public List<TransactionDTO> Transactions { get; set; } = [];

        [JsonConstructor]
        public AccountDTO() { }

        public AccountDTO(Account account) =>
        (Name, ID, Balance, AllowanceTotal, Allowances, Transactions) =
        (
            account.Name,
            account.AccountID,
            account.Balance,
            account.AllowanceBalance,
            DTOUtility.AllowanceListToDTOs(account.Allowances),
            DTOUtility.TransactionListToDTOs(account.Transactions)
        );
    }
}