using System;
using System.Globalization;
using System.Windows.Data;

namespace KipTM.Checks.View
{
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            if (value.GetType() != typeof(double))
                return string.Empty;
            var res = (double)value;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");
            if (value.GetType() != typeof(string))
                return string.Empty;
            var res = (string)value;
            return res;
        }
    }
}
