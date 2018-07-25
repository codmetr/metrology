using System;
using System.Windows.Data;
using System.Windows.Media;

namespace KipTM.Skins.Converters
{
    class BoolToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool) value)
                return new SolidColorBrush(Colors.White);
            return new SolidColorBrush(Colors.Pink);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BoolToColorConverterSelected : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return new SolidColorBrush(Colors.DodgerBlue);
            return new SolidColorBrush(AttractColors(Colors.Blue, Colors.Pink));
        }

        public Color AttractColors(Color col1, Color col2)
        {
            int calcR = AttractChannels(col1.R, col2.R);
            int calcG = AttractChannels(col1.G, col2.G);
            int calcB = AttractChannels(col1.B, col2.B);
            return Color.FromRgb((byte)calcR, (byte)calcG, (byte)calcB);
        }

        public int AttractChannels(byte ch1, byte ch2)
        {
            const int maxChannelValue = 255;
            int calcCh = (ch1 + ch2)/2;
            if (calcCh < 0)
                calcCh = Math.Abs(calcCh);
            if (calcCh > maxChannelValue)
                calcCh = maxChannelValue;
            return calcCh;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
