using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CapacitorScanner.Core.API.Model;
using CapacitorScanner.Core.Model;

namespace CapacitorScanner.Converter
{
    internal class LabelConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is UserModel userModel)
                return $"{userModel.employeename}";
            else if (value is ContainerBinModel container)
                return $"{container.scraptype_name}";
            return "...";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new Avalonia.Data.BindingNotification(value);
        }
    }
}
