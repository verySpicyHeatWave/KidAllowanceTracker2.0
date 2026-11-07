using Avalonia.Data.Converters;
using System;
using System.Globalization;


namespace AllowanceApp.Avalonia.Converters
{
    public class IntegerToDollarString : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString() ?? "nope", out var number)) 
            {
                var calc = number / 100.0;
                var str = Math.Round(calc, 2).ToString("#0.00");
                return $"${str}";
            }
            return "ERROR";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
