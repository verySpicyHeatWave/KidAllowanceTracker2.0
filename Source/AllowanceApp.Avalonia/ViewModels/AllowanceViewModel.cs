using AllowanceApp.Avalonia.Messages;
using AllowanceApp.Avalonia.Models;
using AllowanceApp.Avalonia.Service;
using AllowanceApp.Avalonia.Views;
using AllowanceApp.Shared.Utilities;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AllowanceApp.Avalonia.ViewModels
{
    public class AllowanceViewModel : ViewModelBase
    {
        private readonly int _accountId = 0;
        private int _totalAllowance = 0;
        private static readonly AccountApiCaller _apiCaller = AccountApiCaller.Instance;

        public List<AllowancePoint> PointList { get; set; } = [];

        public IImmutableSolidColorBrush AllowanceColor => _totalAllowance > 0 ? Brushes.LimeGreen : Brushes.Black;

        public string AllowanceDisplay => $"${(_totalAllowance / 100.0):#0.00}";

        public bool HasReportCard => ReportCardIsEntered();

        public string ReportCardBtnText => HasReportCard ? "Edit Report Card" : "Add Report Card";

        public int AllowancePoints => GetPoints(CategoryKeys.BaseAllowance);

        public int GoodPoints => GetPoints(CategoryKeys.GoodPoints);

        public int BadPoints => GetPoints(CategoryKeys.BadPoints);

        public int ChorePoints => GetPoints(CategoryKeys.ChorePoints);

        public int HomeworkPoints => GetPoints(CategoryKeys.HomeworkPoints);

        public int GradeAPoints => GetPoints(CategoryKeys.GradeAPoints);

        public int GradeBPoints => GetPoints(CategoryKeys.GradeBPoints);

        public int GradeCPoints => GetPoints(CategoryKeys.GradeCPoints);

        public int GradeDPoints => GetPoints(CategoryKeys.GradeDPoints);

        public int GradeFPoints => GetPoints(CategoryKeys.GradeFPoints);

        public ICommand AddGoodPointsCommand { get; private set; }

        public ICommand AddBadPointsCommand { get; private set; }

        public ICommand AddChorePointsCommand { get; private set; }

        public ICommand AddHomeworkPointsCommand { get; private set; }

        public ICommand AddReportCardCommand { get; private set; }

        public ICommand PayAllowanceCommand { get; private set; }

        public AllowanceViewModel() : this(0, []) { }

        public AllowanceViewModel(int accountId, List<AllowancePoint> points)
        {
            _accountId = accountId;
            PointList = points;

            AddGoodPointsCommand = new AsyncRelayCommand(OnAddGoodPointsCommand);
            AddBadPointsCommand = new AsyncRelayCommand(OnAddBadPointsCommand);
            AddChorePointsCommand = new AsyncRelayCommand(OnAddChorePointsCommand);
            AddHomeworkPointsCommand = new AsyncRelayCommand(OnAddHomeworkPointsCommand);
            AddReportCardCommand = new AsyncRelayCommand(OnAddReportCardCommand);
            PayAllowanceCommand = new AsyncRelayCommand(OnPayAllowanceCommand);

            UpdateAllProperties();
        }

        private async Task OnPayAllowanceCommand()
        {
            var updatedAccount = await _apiCaller.PayoutAllowance(_accountId);
            foreach (var point in PointList)
            {
                var newPoint = updatedAccount?.AllowancePoints.SingleOrDefault(a => a.Category == point.Category);
                if (newPoint != null)
                {
                    point.Points = newPoint.Points;
                    point.Price = newPoint.Price;
                }
            }
            UpdateAllProperties();
        }

        private async Task OnAddReportCardCommand()
        {
            const int TOTAL_GRADES = 5;
            int validPoints = 0;
            var newPoints = await ReportCardEntryView.ShowDialogAsync(PointList);
            foreach (var kvPair in newPoints)
            {
                var newPoint = await _apiCaller.SetGrade(_accountId, kvPair.Key, kvPair.Value);
                if (newPoint != null)
                {
                    var oldPoint = PointList.SingleOrDefault(a => a.Category == kvPair.Key);
                    oldPoint?.Points = newPoint.Points;
                    validPoints++;
                }
            }   
            UpdateAllProperties();
            if (validPoints == TOTAL_GRADES && HasReportCard)
            {
                WeakReferenceMessenger.Default.Send(new DataRefreshSucessfulMessage());
            }
        }

        private async Task OnAddHomeworkPointsCommand()
        {
            var newPoint = await _apiCaller.IncrementPoint(_accountId, CategoryKeys.HomeworkPoints);
            if (newPoint != null)
            {
                var oldPoint = PointList.SingleOrDefault(a => a.Category == CategoryKeys.HomeworkPoints);
                oldPoint?.Points = newPoint.Points;
                OnPropertyChanged(nameof(HomeworkPoints));
                UpdateSharedProperties();
                WeakReferenceMessenger.Default.Send(new DataRefreshSucessfulMessage());
            }
        }

        private async Task OnAddChorePointsCommand()
        {
            var newPoint = await _apiCaller.IncrementPoint(_accountId, CategoryKeys.ChorePoints);
            if (newPoint != null)
            {
                var oldPoint = PointList.SingleOrDefault(a => a.Category == CategoryKeys.ChorePoints);
                oldPoint?.Points = newPoint.Points;
                OnPropertyChanged(nameof(ChorePoints));
                UpdateSharedProperties();
                WeakReferenceMessenger.Default.Send(new DataRefreshSucessfulMessage());
            }
        }

        private async Task OnAddBadPointsCommand()
        {
            var newPoint = await _apiCaller.IncrementPoint(_accountId, CategoryKeys.BadPoints);
            if (newPoint != null)
            {
                var oldPoint = PointList.SingleOrDefault(a => a.Category == CategoryKeys.BadPoints);
                oldPoint?.Points = newPoint.Points;
                OnPropertyChanged(nameof(BadPoints));
                UpdateSharedProperties();
            }
        }

        private async Task OnAddGoodPointsCommand()
        {
            var newPoint = await _apiCaller.IncrementPoint(_accountId, CategoryKeys.GoodPoints);
            if (newPoint != null)
            {
                var oldPoint = PointList.SingleOrDefault(a => a.Category == CategoryKeys.GoodPoints);
                oldPoint?.Points = newPoint.Points;
                OnPropertyChanged(nameof(GoodPoints));
                UpdateSharedProperties();
                WeakReferenceMessenger.Default.Send(new DataRefreshSucessfulMessage());
            }
        }

        public int GetPoints(string category) =>
            PointList.SingleOrDefault(a => a.Category == category)?.Points ?? -1;

        public int GetPrice(string category) =>
            PointList.SingleOrDefault(a => a.Category == category)?.Price ?? -1;


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

        private void UpdateSharedProperties()
        {
            _totalAllowance = PointList.Sum(t => t.Total);
            OnPropertyChanged(nameof(AllowanceDisplay));
            OnPropertyChanged(nameof(AllowanceColor));
            OnPropertyChanged(nameof(HasReportCard));
            OnPropertyChanged(nameof(ReportCardBtnText));
        }

        private void UpdateAllProperties()
        {
            OnPropertyChanged(nameof(AllowancePoints));
            OnPropertyChanged(nameof(GoodPoints));
            OnPropertyChanged(nameof(BadPoints));
            OnPropertyChanged(nameof(ChorePoints));
            OnPropertyChanged(nameof(HomeworkPoints));
            OnPropertyChanged(nameof(GradeAPoints));
            OnPropertyChanged(nameof(GradeBPoints));
            OnPropertyChanged(nameof(GradeCPoints));
            OnPropertyChanged(nameof(GradeDPoints));
            OnPropertyChanged(nameof(GradeFPoints));
            UpdateSharedProperties();
        }
    }
}
