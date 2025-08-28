using AllowanceApp.Core.Models;
using AllowanceApp.Shared.Utilities;
using System.Text.Json.Serialization;

namespace AllowanceApp.Shared.DTO
{
    public record AccountDTO
    {
        public string Name { get; init; } = string.Empty;
        public int ID { get; init; }
        public int Balance { get; init; }
        public int AllowanceTotal { get; init; }
        public List<AllowancePointDTO> Allowances { get; init; } = [];
        public List<TransactionDTO> Transactions { get; init; } = [];

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