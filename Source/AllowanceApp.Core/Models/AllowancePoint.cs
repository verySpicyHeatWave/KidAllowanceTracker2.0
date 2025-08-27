namespace AllowanceApp.Core.Models
{
    public class AllowancePoint(string Category, int Price)
    {
        public int AccountID { get; set; } = 0;
        public string Category { get; set; } = Category;
        public int Price { get; set; } = Price;
        public int Points { get; set; } = 0;
        public int Total => Price * Points;
    }
}
