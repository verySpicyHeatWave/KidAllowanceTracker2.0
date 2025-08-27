using AllowanceApp.Core.Models;
using AllowanceApp.Api.Utilities;
using AllowanceApp.Core.Utilities;

namespace AllowanceApp.Api.DTO
{
    public record AccountDTO(string Name, int ID, int Balance, int AllowanceTotal, List<AllowancePointDTO> Allowances, List<TransactionDTO> Transactions)
    {
        public AccountDTO(Account account) : this
        (
            account.Name,
            account.AccountID,
            account.Balance,
            account.AllowanceBalance,
            DTOUtility.AllowanceListToDTOs(account.Allowances),
            DTOUtility.TransactionListToDTOs(account.Transactions)
        ) { }
    }
}