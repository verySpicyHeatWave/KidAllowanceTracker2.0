using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Blazor.Models
{
    public class TransactionsViewModel
    {
        public List<SingleTransactionModel> TransactionList { get; set; } = [];


        public TransactionsViewModel(List<TransactionDTO> dtos)
        {
            foreach (var dto in dtos)
            {
                TransactionList.Add(new SingleTransactionModel(dto));
            }
        }
    }
}
