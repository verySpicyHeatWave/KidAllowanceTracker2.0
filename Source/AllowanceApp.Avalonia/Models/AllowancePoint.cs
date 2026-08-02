using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Avalonia.Models
{
    public class AllowancePoint(AllowancePointDTO dto)
    {
        public int AccountID { get; set; } = dto.AccountID;
        public string Category { get; set; } = dto.Category;
        public int Price { get; set; } = dto.Price;
        public int Points { get; set; } = dto.Points;
        public int Total => Price * Points;
    }
}
