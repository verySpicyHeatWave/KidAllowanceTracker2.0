using AllowanceApp.Core.Models;
using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Shared.Utilities
{
    public static class DTOUtility
    {
        public static List<AccountDTO> AccountListToDTOs(List<Account> accounts)
        {
            List<AccountDTO> Response = [];
            foreach (var account in accounts)
            {
                Response.Add(new AccountDTO(account));
            }
            return Response;
        }
        public static List<AllowancePointDTO> AllowanceListToDTOs(List<AllowancePoint> allowances)
        {
            List<AllowancePointDTO> Response = [];
            foreach (var APoint in allowances)
            {
                Response.Add(new AllowancePointDTO(APoint));
            }
            return Response;
        }

        public static List<TransactionDTO> TransactionListToDTOs(List<Transaction> transactions)
        {
            List<TransactionDTO> Response = [];
            foreach (var transaction in transactions)
            {
                Response.Add(new TransactionDTO(transaction));
            }
            return Response;
        }
    }
}