using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AllowanceApp.Avalonia.Converters
{
    public class IntegerToDollarColor : IValueConverter
    {
        private static readonly Color Unknown = Colors.Black;
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Color resp = Unknown;
            if (int.TryParse(value?.ToString() ?? "nope", out var number) && number > 0)
            {
                 resp = Colors.LimeGreen;
            }
            return new SolidColorBrush(resp);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
