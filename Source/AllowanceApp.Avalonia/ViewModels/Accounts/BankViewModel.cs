using AllowanceApp.Avalonia.Models;
using AllowanceApp.Avalonia.Service;
using AllowanceApp.Shared.DTO;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.ViewModels.Accounts
{
    public class BankViewModel : RefreshableViewModelBase
    {
        private int _id;
        private int _balance;

        public int Balance => _balance;
        public IImmutableSolidColorBrush BalanceDisplayColor => _balance > 0 ? Brushes.LimeGreen : Brushes.Black;
        public string AmountEntry { get; set; } = string.Empty; 
        public string DescriptionEntry { get; set; } = string.Empty;
        public AsyncRelayCommand RequestDeposit { get; private set; }
        public AsyncRelayCommand RequestWithdrawal { get; private set; }
        public BankViewModel(int id, int balance)
        {
            Width = 450;
            Height = 700;
            _balance = balance;
            _id = id;
            RequestDeposit = new AsyncRelayCommand(OnRequestDeposit);
            RequestWithdrawal = new AsyncRelayCommand(OnRequestWithdrawal);
        }

        public BankViewModel() : this(0, 0) { }

        private async Task OnRequestWithdrawal()
        {
            int value = GetAmountFromTextBox();
            if ( value < 1)
            {
                var msgbox = MessageBoxManager.GetMessageBoxStandard(
                    "Uh-Oh!",
                    $"{AmountEntry} is not a valid number!",
                    ButtonEnum.Ok,
                    Icon.Warning);

                await msgbox.ShowAsync();
                return;
            }

            if (value > _balance)
            {
                var msgbox = MessageBoxManager.GetMessageBoxStandard(
                    "Uh-Oh!",
                    $"{AmountEntry} is more than the amount you have!",
                    ButtonEnum.Ok,
                    Icon.Warning);

                await msgbox.ShowAsync();
                return;
            }
            var transaction = new TransactionRequest(value, DescriptionEntry);
            await AccountApiCaller.Instance.RequestTransaction(_id, "withdrawal", transaction);
        }

        private async Task OnRequestDeposit()
        {
            int value = GetAmountFromTextBox();
            if (value < 1)
            {
                var msgbox = MessageBoxManager.GetMessageBoxStandard(
                    "Uh-Oh!",
                    $"{AmountEntry} is not a valid number!",
                    ButtonEnum.Ok,
                    Icon.Warning);

                await msgbox.ShowAsync();
                return;
            }

            var transaction = new TransactionRequest(value, DescriptionEntry);
            await AccountApiCaller.Instance.RequestTransaction(_id, "deposit", transaction);
        }


        public override void Refresh(Account account)
        {
            _balance = account.Balance;
            OnPropertyChanged(nameof(Balance));
        }

        private int GetAmountFromTextBox()
        {
            if (double.TryParse(AmountEntry, out var val))
            {
                return (int)(val * 100);
            }
            return int.MinValue;
        }
    }
}
