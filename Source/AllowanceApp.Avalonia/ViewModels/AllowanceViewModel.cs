using AllowanceApp.Avalonia.Models;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace AllowanceApp.Avalonia.ViewModels
{
    public partial class AllowanceViewModel : ViewModelBase
    {
        private int _totalAllowance = 0;

        public List<AllowancePoint> PointList { get; set; } = [];

        [ObservableProperty]
        public partial IImmutableSolidColorBrush AllowanceColor { get; set; }

        [ObservableProperty]
        public partial string AllowanceDisplay { get; set; }

        [ObservableProperty]
        public partial int AllowancePoints { get; set; }

        [ObservableProperty]
        public partial int GoodPoints { get; set; }

        [ObservableProperty]
        public partial int BadPoints { get; set; }

        [ObservableProperty]
        public partial int ChorePoints { get; set; }

        [ObservableProperty]
        public partial int HomeworkPoints { get; set; }

        [ObservableProperty]
        public partial int GradeAPoints { get; set; }

        [ObservableProperty]
        public partial int GradeBPoints { get; set; }

        [ObservableProperty]
        public partial int GradeCPoints { get; set; }

        [ObservableProperty]
        public partial int GradeDPoints { get; set; }

        [ObservableProperty]
        public partial int GradeFPoints { get; set; }

        [ObservableProperty]
        public partial bool HasReportCard { get; set; }

        [ObservableProperty]
        public partial string ReportCardBtnText { get; set; }

        public double TotalAllowance => CalculateTotal();

        public AllowanceViewModel() : this([]) { }

        public AllowanceViewModel(List<AllowancePoint> points)
        {
            PointList = points;
            AllowanceDisplay = $"${CalculateTotal():#0.00}";
            AllowanceColor = _totalAllowance > 0 ? Brushes.LimeGreen : Brushes.Black;

            HasReportCard = ReportCardIsEntered();
            ReportCardBtnText = HasReportCard ? "Edit Report Card" : "Add Report Card";

            AllowancePoints = GetPoints("BaseAllowance");
            GoodPoints = GetPoints("GoodBehavior");
            BadPoints = GetPoints("BadBehavior");
            ChorePoints = GetPoints("Chores");
            HomeworkPoints = GetPoints("Homework");
            GradeAPoints = GetPoints("GradeA");
            GradeBPoints = GetPoints("GradeB");
            GradeCPoints = GetPoints("GradeC");
            GradeDPoints = GetPoints("GradeD");
            GradeFPoints = GetPoints("GradeF");
        }

        public int GetPoints(string category) =>
            PointList.SingleOrDefault(a => a.Category == category)?.Points ?? -1;

        public int GetPrice(string category) =>
            PointList.SingleOrDefault(a => a.Category == category)?.Price ?? -1;

        private double CalculateTotal()
        {
            _totalAllowance = PointList.Sum(t => t.Total);
            return _totalAllowance / 100.0;
        }

        private bool ReportCardIsEntered()
        {
            return PointList.Any(a =>
                    (a.Category == "GradeA" && a.Points > 0)
                 || (a.Category == "GradeB" && a.Points > 0)
                 || (a.Category == "GradeC" && a.Points > 0)
                 || (a.Category == "GradeD" && a.Points > 0)
                 || (a.Category == "GradeF" && a.Points > 0)
            );
        }
    }
}
