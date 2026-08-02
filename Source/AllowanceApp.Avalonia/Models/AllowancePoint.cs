using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Avalonia.Models
{
    public class AllowancePoint(AllowancePointDTO dto)
    {
        public int AccountID { get; init; } = dto.AccountID;
        public string Category { get; init; } = dto.Category;
        public int Price { get; init; } = dto.Price;
        public int Points { get; init; } = dto.Points;
        public int Total => Price * Points;
    }
}
