﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace FMStudio.App.Converters
{
    [ValueConversion(typeof(bool), typeof(double))]
    public class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? 0.5 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}