using System;
using System.Globalization;
using System.Windows.Data;
using PressureSensorCheck.Workflow;
using KipTM.Interfaces;
using PressureSensorData;

namespace KipTM.Checks.View
{
    [ValueConversion(typeof(PressureSensorPointConf), typeof(string))]
    public class PointConfigConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            var conf = value as PressureSensorPointConf;
            if (conf == null)
                return string.Empty;
            var unit = conf.PressureUnit.ToStringLocalized(CultureInfo.CurrentUICulture);
            var unitOut = conf.OutUnit.ToStringLocalized(CultureInfo.CurrentUICulture);
            return $"{conf.PressurePoint} {unit} (I = {conf.OutPoint} {unitOut})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
