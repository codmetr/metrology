﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KipTM.Skins.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        private BooleanToVisibilityConverter _converter = new BooleanToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = (Visibility)_converter.Convert(value, targetType, parameter, culture);
            res = res == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = (bool)_converter.ConvertBack(value, targetType, parameter, culture);
            res = !res;
            return res;
        }
    }
}
