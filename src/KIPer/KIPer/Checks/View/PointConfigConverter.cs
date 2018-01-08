using System;
using System.Globalization;
using System.Windows.Data;
using PressureSensorCheck.Workflow;
using KipTM.Interfaces;

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
            var unit = conf.Unit.ToStringLocalized(CultureInfo.CurrentUICulture);
            return $"{conf.Pressure} {unit} (I = {conf.I} мА)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
