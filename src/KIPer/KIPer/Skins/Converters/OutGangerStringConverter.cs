using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using KipTM.Checks.ViewModel.Config;
using PressureSensorCheck.Workflow;

namespace KipTM.Skins.Converters
{
    [ValueConversion(typeof(OutGange), typeof(string))]
    public class OutGangerStringConverter : IValueConverter
    {
        public static Dictionary<OutGange, string> Default = new Dictionary<OutGange, string>()
        {
            {OutGange.I0_5mA, "0-5 мА" },
            {OutGange.I4_20mA, "4-20 мА" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(string))
            //    throw new InvalidOperationException("The target must be a string");
            if (value.GetType() != typeof(OutGange))
                throw new InvalidOperationException("The value must be a OutGange");

            return (object)Default[(OutGange)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return default(OutGange);
            if (value.GetType() == typeof(OutGange))
                return (OutGange)value;
            if (value.GetType() != typeof(string))
                throw new InvalidOperationException("The value must be a string");
            if (targetType != typeof(OutGange))
                throw new InvalidOperationException("The target must be a OutGange");

            return Default.FirstOrDefault(el=>el.Value == (string)value).Key;
        }
    }
}