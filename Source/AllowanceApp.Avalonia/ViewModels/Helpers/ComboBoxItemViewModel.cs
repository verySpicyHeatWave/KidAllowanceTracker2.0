namespace AllowanceApp.Avalonia.ViewModels.Helpers
{
    public class ComboBoxItemViewModel<T>(string text, T value)
    {
        public string Text { get; set; } = text;
        public T Value { get; } = value;
    }
}
