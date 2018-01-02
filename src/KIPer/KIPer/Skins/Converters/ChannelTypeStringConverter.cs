using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using ArchiveData.DTO;
using PressureSensorData;

namespace KipTM.Skins.Converters
{
    [ValueConversion(typeof(ChannelType), typeof(string))]
    public class ChannelTypeStringConverter : IValueConverter
    {
        private static readonly Dictionary<ChannelType, string> Default = new Dictionary<ChannelType, string>()
        {
            {ChannelType.Temperature, "температура" },
            {ChannelType.Current, "ток" },
            {ChannelType.Pressure, "давление" },
            {ChannelType.Voltage, "напряжение" },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || targetType == typeof(string))
                return null;
            //    throw new InvalidOperationException("The target must be a string");
            if (value.GetType() != typeof(ChannelType))
                return null;
                //throw new InvalidOperationException("The value must be a ChannelType");

            return (object)Default[(ChannelType)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return default(ChannelType);
            if (value.GetType() != typeof(string))
                throw new InvalidOperationException("The value must be a string");
            if (targetType != typeof(ChannelType))
                throw new InvalidOperationException("The target must be a ChannelType");

            return Default.FirstOrDefault(el=>el.Value == (string)value).Key;
        }
    }
}