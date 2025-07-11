using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CapacitorScanner.Converter
{
    internal class ProgressBarColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double progress )
            {
                if (progress < 30)
                    return Brushes.Green;
                else if (progress <  70)
                    return Brushes.Orange;
                return Brushes.Red;
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new Avalonia.Data.BindingNotification(value);
        }
    }
}
