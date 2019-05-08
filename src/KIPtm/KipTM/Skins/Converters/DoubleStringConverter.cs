using System;
using System.Globalization;
using System.Windows.Data;

namespace KipTM.Skins.Converters
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
            var resString = (string)value;
            double res;
            if (!double.TryParse(resString.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out res))
                return double.NaN;
            return res;
        }
    }
}
