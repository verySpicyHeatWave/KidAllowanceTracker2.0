using CommunityToolkit.Mvvm.ComponentModel;

namespace AllowanceApp.Avalonia.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public int Width { get; set; }
    public int Height { get; set; }
}
