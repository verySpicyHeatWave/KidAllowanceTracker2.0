using AllowanceApp.Avalonia.ViewModels;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AllowanceApp.Avalonia.Converters
{
    internal class IntegerToTransactionStatusItalics : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            FontStyle resp = FontStyle.Normal;

            if (value == null) return resp;

            if (Enum.IsDefined(typeof(CategoryFilterEnum), value))
            {
                if ((CategoryFilterEnum)value == CategoryFilterEnum.Pending)
                    resp = FontStyle.Italic;
            }
            return resp;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
