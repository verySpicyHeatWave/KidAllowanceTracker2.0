using AllowanceApp.Avalonia.Models;
using AllowanceApp.Shared.Utilities;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Views
{
    public partial class ReportCardEntryView : Window
    {
        private Dictionary<string, int> allowancePoints;

        public static async Task<Dictionary<string, int>> ShowDialogAsync(List<AllowancePoint> allowancePoints)
        {
            var dialog = new ReportCardEntryView(allowancePoints);
            return await dialog.ShowDialog<Dictionary<string, int>>(MainWindow.Instance!);
        }

        public ReportCardEntryView()
        {
            InitializeComponent();
            this.allowancePoints = [];
        }

        private ReportCardEntryView(List<AllowancePoint> allowancePoints)
        {
            InitializeComponent();

            this.allowancePoints = new()
            {
                { CategoryKeys.GradeAPoints, allowancePoints.SingleOrDefault(p => p.Category == CategoryKeys.GradeAPoints)?.Points ?? 0 },
                { CategoryKeys.GradeBPoints, allowancePoints.SingleOrDefault(p => p.Category == CategoryKeys.GradeBPoints)?.Points ?? 0 },
                { CategoryKeys.GradeCPoints, allowancePoints.SingleOrDefault(p => p.Category == CategoryKeys.GradeCPoints)?.Points ?? 0 },
                { CategoryKeys.GradeDPoints, allowancePoints.SingleOrDefault(p => p.Category == CategoryKeys.GradeDPoints)?.Points ?? 0 },
                { CategoryKeys.GradeFPoints, allowancePoints.SingleOrDefault(p => p.Category == CategoryKeys.GradeFPoints)?.Points ?? 0 }
            };

            GradeAUpDown.Value = this.allowancePoints[CategoryKeys.GradeAPoints];
            GradeBUpDown.Value = this.allowancePoints[CategoryKeys.GradeBPoints];
            GradeCUpDown.Value = this.allowancePoints[CategoryKeys.GradeCPoints];
            GradeDUpDown.Value = this.allowancePoints[CategoryKeys.GradeDPoints];
            GradeFUpDown.Value = this.allowancePoints[CategoryKeys.GradeFPoints];
        }

        private void OnSubmitClick(object? sender, RoutedEventArgs e)
        {
            allowancePoints[CategoryKeys.GradeAPoints] = (int)GradeAUpDown.Value!;
            allowancePoints[CategoryKeys.GradeBPoints] = (int)GradeBUpDown.Value!;
            allowancePoints[CategoryKeys.GradeCPoints] = (int)GradeCUpDown.Value!;
            allowancePoints[CategoryKeys.GradeDPoints] = (int)GradeDUpDown.Value!;
            allowancePoints[CategoryKeys.GradeFPoints] = (int)GradeFUpDown.Value!;
            Close(allowancePoints);
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            Close(allowancePoints);
        }
    }
}