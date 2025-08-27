using System.Diagnostics.CodeAnalysis;

namespace AllowanceApp.Core.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public required string Name { get; set; }
        public int Balance { get; set; } = 0;
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
                new AllowancePoint("BaseAllowance", 500) { Points = 1 },
                new AllowancePoint("GoodBehavior", 100),
                new AllowancePoint("Homework", 100),
                new AllowancePoint("Chores", 100),
                new AllowancePoint("BadBehavior", -50),
                new AllowancePoint("GradeA", 500),
                new AllowancePoint("GradeB", 300),
                new AllowancePoint("GradeC", 100),
                new AllowancePoint("GradeD", 0),
                new AllowancePoint("GradeF", -50)
            ];

        }
    }
}
