using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CapacitorScanner.Converter
{
    internal class ProgressBarWidthConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            double val = 1.5;
            if (value is decimal width)
                return width / (decimal)val;
            else if (value is double w)
                return w / val;
            else if (value is int w2)
                return w2 / val;
            else
                return val;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new Avalonia.Data.BindingNotification(value);
        }
    }
}
