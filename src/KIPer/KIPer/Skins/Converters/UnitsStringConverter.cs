using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using KipTM.Interfaces;

namespace KipTM.Skins.Converters
{
    [ValueConversion(typeof(Units), typeof(string))]
    public class UnitsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)// || targetType != typeof(string)
                return null;
            //    throw new InvalidOperationException("The target must be a string");
            if (value.GetType() != typeof(Units))
                return null;
            //throw new InvalidOperationException("The value must be a Units");

            return ((Units)value).ToStringLocalized(CultureInfo.CurrentUICulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return default(Units);
            if (value.GetType() != typeof(string))
                throw new InvalidOperationException("The value must be a string");
            if (targetType != typeof(Units))
                throw new InvalidOperationException("The target must be a Units");

            return UnitDict.ParceLocalized((string)value, CultureInfo.CurrentUICulture);
        }
    }
}