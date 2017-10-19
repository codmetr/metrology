using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using KipTM.Checks.ViewModel.Config;

namespace KipTM.Checks.View
{
    [ValueConversion(typeof(PointConfigViewModel), typeof(string))]
    public class PointConfigConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            var conf = value as PointConfigViewModel;
            if (conf == null)
                return string.Empty;
            return $"{conf.Pressire} {conf.Unit}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
