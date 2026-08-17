using System;
using System.Collections.Generic;
using System.Text;

namespace AllowanceApp.Avalonia.ViewModels
{
    public class ComboBoxItemViewModel<T>(string text, T value)
    {
        public string Text { get; set; } = text;
        public T Value { get; } = value;
    }
}
