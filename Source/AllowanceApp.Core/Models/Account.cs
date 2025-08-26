using System.Diagnostics.CodeAnalysis;

namespace AllowanceApp.Core.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public required string Name { get; set; }
        public double Balance { get; set; } = 0;
        public List<AllowancePoint> Allowances { get; set; } = [];
        public List<Transaction> Transactions { get; set; } = [];

        protected Account() { }

        [SetsRequiredMembers] 
        public Account(string name)
        {
            Name = name;
            PopulateDefaultAllowancePoints();
        }

        private void PopulateDefaultAllowancePoints()
        {
            Allowances =
            [
                new AllowancePoint("BaseAllowance", 5) { Points = 1 },
                new AllowancePoint("GoodBehavior", 1),
                new AllowancePoint("Homework", 1),
                new AllowancePoint("Chores", 1),
                new AllowancePoint("BadBehavior", -0.5),
                new AllowancePoint("GradeA", 5),
                new AllowancePoint("GradeB", 3),
                new AllowancePoint("GradeC", 1),
                new AllowancePoint("GradeD", 0),
                new AllowancePoint("GradeF", -0.5)
            ];

        }
    }
}
