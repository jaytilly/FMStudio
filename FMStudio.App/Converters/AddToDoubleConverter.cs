using System;
using System.Globalization;
using System.Windows.Data;

namespace FMStudio.App.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class AddToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterAsDouble = double.Parse((string)parameter);
            return (double)value + parameterAsDouble;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}