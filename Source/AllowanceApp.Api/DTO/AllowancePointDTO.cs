using AllowanceApp.Core.Models;

namespace AllowanceApp.Api.DTO
{
    public record AllowancePointDTO(int AccountID, string Category, double Price, double Points, double Total)
    {
        public AllowancePointDTO(AllowancePoint APoint) : this
        (
            APoint.AccountID,
            APoint.Category,
            APoint.Price,
            APoint.Points,
            APoint.Total
        ) { }
    }
}
