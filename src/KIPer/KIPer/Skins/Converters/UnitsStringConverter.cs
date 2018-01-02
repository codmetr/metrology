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
        public static readonly Dictionary<Units, string> Default = new Dictionary<Units, string>()
        {
            {Units.bar, "бар" },
            {Units.kgSm, "кгСм" },
            {Units.mA, "мА" },
            {Units.A, "А" },
            {Units.mV, "мВ" },
            {Units.V, "М" },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || targetType == typeof(string))
                return null;
            //    throw new InvalidOperationException("The target must be a string");
            if (value.GetType() != typeof(Units))
                return null;
            //throw new InvalidOperationException("The value must be a Units");

            return (object)Default[(Units)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return default(Units);
            if (value.GetType() != typeof(string))
                throw new InvalidOperationException("The value must be a string");
            if (targetType != typeof(Units))
                throw new InvalidOperationException("The target must be a Units");

            return Default.FirstOrDefault(el=>el.Value == (string)value).Key;
        }
    }
}