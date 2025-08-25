namespace AllowanceApp.Core.Models
{
    public class AllowancePoint(string Category, double Price)
    {
        public int AccountID { get; set; } = 0;
        public string Category { get; set; } = Category;
        public double Price { get; set; } = Price;
        public int Points { get; set; } = 0;
        public double Total => Price * Points;
    }
}
