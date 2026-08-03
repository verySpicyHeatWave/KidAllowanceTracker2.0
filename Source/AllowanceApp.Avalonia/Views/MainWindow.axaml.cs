using Avalonia.Controls;

namespace AllowanceApp.Avalonia.Views;

public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
    }
}