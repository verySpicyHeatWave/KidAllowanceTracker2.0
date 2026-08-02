using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AllowanceApp.Avalonia.ViewModels;
using AllowanceApp.Avalonia.Views;
using AllowanceApp.Avalonia.Service;

namespace AllowanceApp.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        // populating the accounts cache before loading the application
        AccountApiCaller.Instance.GetAllAccounts();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}