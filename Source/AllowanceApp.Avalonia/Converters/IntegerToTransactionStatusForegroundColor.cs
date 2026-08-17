using AllowanceApp.Avalonia.ViewModels;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AllowanceApp.Avalonia.Converters
{
    internal class IntegerToTransactionStatusForegroundColor : IValueConverter
    {
        private static readonly Color Unknown = Colors.Black;
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Color resp = Unknown;

            if (value == null) return resp;

            if (Enum.IsDefined(typeof(CategoryFilterEnum), value))
            {
                resp = (CategoryFilterEnum)value switch
                {
                    CategoryFilterEnum.Approved => Colors.LimeGreen,
                    CategoryFilterEnum.Declined => Colors.Red,
                    _ => Unknown

                };
            }
            return new SolidColorBrush(resp);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
