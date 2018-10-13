using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PACESeries;
using PACESeriesUtil.Converters;

namespace PACESeriesUtil.PaceControl
{
    [ValueConversion(typeof(PressureUnits), typeof(string))]
    public class PressureUnitsToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            if (value == null)
                throw new InvalidOperationException("The sounce is null");
            return ((PressureUnits) value).ToLocalizedString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
