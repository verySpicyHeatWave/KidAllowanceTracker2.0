using AllowanceApp.Avalonia.ViewModels;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AllowanceApp.Avalonia.Converters
{
    public class IntegerToTransactionStatus : IValueConverter
    {
        private const string Unknown = "Unknown";
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string resp = Unknown;

            if (value == null) return resp;

            if (Enum.IsDefined(typeof(CategoryFilterEnum), value))
            {
                resp = ((CategoryFilterEnum)value).ToString();
            }
            return resp;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
