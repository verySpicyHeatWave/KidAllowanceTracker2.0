using AllowanceApp.Avalonia.Models;
using AllowanceApp.Avalonia.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial ViewModelBase CurrentView { get; set; }

    private ConfigManager _configManager;
    private bool _askForAccount;

    public MainViewModel()
    {
        _configManager = new ConfigManager("Account.json");

        int accountNumber = _configManager.GetProperty("AccountNumber", -1);
        _askForAccount = _configManager.GetProperty("AlwaysAskForAccount", false);

        // If this fails because of a bad HTTP response, we need to catch that exception and set the current viewmodel to
        // an error viewmodel that displays the error message, an retry every thirty seconds until we get a valid response from the server.
        // Or maybe I'll bring the notification system over from the STC app and use that to display error messages, that'd be better, maybe.
        var api = AccountApiCaller.Instance;
        var accounts = api.GetAllAccountsAsync().Result;
        bool accountExists = accounts.Exists(a => a.ID == accountNumber);


        if (_askForAccount || !accountExists)
        {
            var vm = new UserSelectionViewModel(accounts);
            vm.AccountConfirmed += OnAccountConfirmed;
            CurrentView = vm;
        }
        else
        {
            var account = accounts.First(acct => acct.ID == accountNumber);
            CurrentView = new AccountViewModel(account);
        }

    }

    private void OnAccountConfirmed(object? sender, Account e)
    {
        if (CurrentView is UserSelectionViewModel vm)
        {
            vm.AccountConfirmed -= OnAccountConfirmed;
            CurrentView = new AccountViewModel(e);
            if (!_askForAccount)
            {
                _configManager.SetProperty("AccountNumber", e.ID, true);
            }
        }
    }
}
