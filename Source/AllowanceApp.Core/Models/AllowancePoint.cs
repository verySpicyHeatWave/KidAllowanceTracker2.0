namespace AllowanceApp.Core.Models
{
    public enum PointOperation
    {
        Decrement = -1,
        Increment = 1
    }
    public class AllowancePoint(string Category, int Price)
    {
        public int AccountID { get; set; } = 0;
        public string Category { get; set; } = Category;
        public int Price { get; set; } = Price;
        public int Points { get; set; } = 0;
        public int Total => Price * Points;

        public void IncOrDecPoint(PointOperation incOrDec)
        {
            Points += (int)incOrDec;
            if (Points < 0) { Points = 0; }
        }
    }



}
