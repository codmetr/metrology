using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KipTM.View.Converters
{
    public class DoubleToTimespanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new Exception(string.Format("targetType ({0}) is not {1}", targetType, typeof(string)));
            }

            if (value.GetType() != typeof(TimeSpan))
                throw new Exception(string.Format("value type ({0}) is not {1}", value.GetType(), typeof(TimeSpan)));

            TimeSpan valueTs = (TimeSpan)value;

            return valueTs.TotalMilliseconds.ToString("f0");
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (targetType != typeof(TimeSpan))
            {
                throw new Exception(string.Format("targetType ({0}) is not {1}", targetType, typeof(TimeSpan)));
            }

            if (value.GetType() != typeof(string))
                throw new Exception(string.Format("value type ({0}) is not {1}", value.GetType(), typeof(string)));

            double valueMs;
            if (!double.TryParse(value.ToString(), out valueMs))
                throw new Exception(string.Format("Can't parse double from \"{0}\"", value.ToString()));

            return TimeSpan.FromMilliseconds(valueMs);
        }
    }
}
