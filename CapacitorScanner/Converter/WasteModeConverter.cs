using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CapacitorScanner.Converter
{
    internal class WasteModeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool mode)
                return $"Mode: {(mode ? "Auto" : "Manual")}";
            return new Avalonia.Data.BindingNotification(value);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new Avalonia.Data.BindingNotification(value);
        }
    }
}
