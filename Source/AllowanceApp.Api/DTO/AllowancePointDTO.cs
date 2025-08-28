using System.Text.Json.Serialization;
using AllowanceApp.Core.Models;

namespace AllowanceApp.Api.DTO
{
    public record AllowancePointDTO
    {
        public int AccountID { get; init; }
        public string Category { get; init; } = string.Empty;
        public int Price { get; init; }
        public double Points { get; init; }
        public double Total { get; init; }

        [JsonConstructor]
        public AllowancePointDTO() {}

        public AllowancePointDTO(AllowancePoint APoint) =>
        (AccountID, Category, Price, Points, Total) =
        (
            APoint.AccountID,
            APoint.Category,
            APoint.Price,
            APoint.Points,
            APoint.Total
        );
    }
}
