using AllowanceApp.Avalonia.Models;
using AllowanceApp.Avalonia.Service;
using AllowanceApp.Shared.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AllowanceApp.Avalonia.ViewModels.Admin
{
    public partial class PointPricesViewModel : ViewModelBase
    {
        private int _id = 0;

        [ObservableProperty]
        public partial double GoodBehaviorPrice { get; set; }

        [ObservableProperty]
        public partial int GoodBehaviorPoints { get; set; }

        [ObservableProperty]
        public partial double BadBehaviorPrice { get; set; }

        [ObservableProperty]
        public partial int BadBehaviorPoints { get; set; }

        [ObservableProperty]
        public partial double HomeworkPrice { get; set; }

        [ObservableProperty]
        public partial int HomeworkPoints { get; set; }

        [ObservableProperty]
        public partial double ChoresPrice { get; set; }

        [ObservableProperty]
        public partial int ChoresPoints { get; set; }

        [ObservableProperty]
        public partial double AGradesPrice { get; set; }

        [ObservableProperty]
        public partial int AGradesPoints { get; set; }

        [ObservableProperty]
        public partial double BGradesPrice { get; set; }

        [ObservableProperty]
        public partial int BGradesPoints { get; set; }

        [ObservableProperty]
        public partial double CGradesPrice { get; set; }

        [ObservableProperty]
        public partial int CGradesPoints { get; set; }

        [ObservableProperty]
        public partial double DGradesPrice { get; set; }

        [ObservableProperty]
        public partial int DGradesPoints { get; set; }

        [ObservableProperty]
        public partial double FGradesPrice { get; set; }

        [ObservableProperty]
        public partial int FGradesPoints { get; set; }

        [ObservableProperty]
        public partial ICommand SavePrices { get; set; }

        public PointPricesViewModel() : this(new Account()) { }

        public PointPricesViewModel(Account acct)
        {
            SavePrices = new AsyncRelayCommand(OnSavePrices);

            Width = 700;
            Height = 700;

            UpdatePricesUi(acct);
        }

        private async Task OnSavePrices()
        {
            var newAcct = await AccountApiCaller.Instance.GetAccountAsync(_id);
            if (newAcct is null) return;

            foreach (var point in newAcct.AllowancePoints)
            {
                switch (point.Category)
                {
                    case CategoryKeys.GoodPoints:
                        point.Price = (int)(GoodBehaviorPrice * 100);
                        point.Points = GoodBehaviorPoints;
                        break;
                    case CategoryKeys.BadPoints:
                        point.Price = (int)(BadBehaviorPrice * 100);
                        point.Points = BadBehaviorPoints;
                        break;
                    case CategoryKeys.HomeworkPoints:
                        point.Price = (int)(HomeworkPrice * 100);
                        point.Points = HomeworkPoints;
                        break;
                    case CategoryKeys.ChorePoints:
                        point.Price = (int)(ChoresPrice * 100);
                        point.Points = ChoresPoints;
                        break;
                    case CategoryKeys.GradeAPoints:
                        point.Price = (int)(AGradesPrice * 100);
                        point.Points = AGradesPoints;
                        break;
                    case CategoryKeys.GradeBPoints:
                        point.Price = (int)(BGradesPrice * 100);
                        point.Points = BGradesPoints;
                        break;
                    case CategoryKeys.GradeCPoints:
                        point.Price = (int)(CGradesPrice * 100);
                        point.Points = CGradesPoints;
                        break;
                    case CategoryKeys.GradeDPoints:
                        point.Price = (int)(DGradesPrice * 100);
                        point.Points = DGradesPoints;
                        break;
                    case CategoryKeys.GradeFPoints:
                        point.Price = (int)(FGradesPrice * 100);
                        point.Points = FGradesPoints;
                        break;
                }

                await AccountApiCaller.Instance.SetPrice(_id, point.Category, point.Price);
                await AccountApiCaller.Instance.SetPoints(_id, point.Category, point.Points);
            }
        }

        private void UpdatePricesUi(Account acct)
        {
            foreach (var point in acct.AllowancePoints)
            {
                switch (point.Category)
                {
                    case CategoryKeys.GoodPoints:
                        GoodBehaviorPrice = Math.Round(point.Price / 100.0, 2);
                        GoodBehaviorPoints = point.Points;
                        break;
                    case CategoryKeys.BadPoints:
                        BadBehaviorPrice = Math.Round(point.Price / 100.0, 2);
                        BadBehaviorPoints = point.Points;
                        break;
                    case CategoryKeys.HomeworkPoints:
                        HomeworkPrice = Math.Round(point.Price / 100.0, 2);
                        HomeworkPoints = point.Points;
                        break;
                    case CategoryKeys.ChorePoints:
                        ChoresPrice = Math.Round(point.Price / 100.0, 2);
                        ChoresPoints = point.Points;
                        break;
                    case CategoryKeys.GradeAPoints:
                        AGradesPrice = Math.Round(point.Price / 100.0, 2);
                        AGradesPoints = point.Points;
                        break;
                    case CategoryKeys.GradeBPoints:
                        BGradesPrice = Math.Round(point.Price / 100.0, 2);
                        BGradesPoints = point.Points;
                        break;
                    case CategoryKeys.GradeCPoints:
                        CGradesPrice = Math.Round(point.Price / 100.0, 2);
                        CGradesPoints = point.Points;
                        break;
                    case CategoryKeys.GradeDPoints:
                        DGradesPrice = Math.Round(point.Price / 100.0, 2);
                        DGradesPoints = point.Points;
                        break;
                    case CategoryKeys.GradeFPoints:
                        FGradesPrice = Math.Round(point.Price / 100.0, 2);
                        FGradesPoints = point.Points;
                        break;
                }
            }

        }
    }
}
