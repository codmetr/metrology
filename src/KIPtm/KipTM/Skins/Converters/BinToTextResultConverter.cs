using System;
using System.Globalization;
using System.Windows.Data;

namespace KipTM.Skins.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class BinToTextResultConverter:IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Преобразует значение. 
        /// </summary>
        /// <returns>
        /// Преобразованное значение.Если метод возвращает null, используется действительное значение null.
        /// </returns>
        /// <param name="value">Значение, произведенное исходной привязкой.</param>
        /// <param name="targetType">Тип свойства цели связывания.</param>
        /// <param name="parameter">Параметр используемого преобразователя.</param>
        /// <param name="culture">Язык и региональные параметры, используемые в преобразователе.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string) && targetType != typeof(object))
                return null;
            if(value.GetType() != typeof(bool))
                return null;
            var boolVal = (bool) value;
            return boolVal ? "Исправен" : "Неисправен";
        }

        /// <summary>
        /// Преобразует значение. 
        /// </summary>
        /// <returns>
        /// Преобразованное значение.Если метод возвращает null, используется действительное значение null.
        /// </returns>
        /// <param name="value">Значение, произведенное целью привязки.</param>
        /// <param name="targetType">Тип, к которому выполняется преобразование.</param>
        /// <param name="parameter">Используемый параметр преобразователя.</param>
        /// <param name="culture">Язык и региональные параметры, используемые в преобразователе.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(targetType != typeof(bool))
                return null;
            if (value.GetType() != typeof(string))
                return null;
            var stringVal = (string)value;
            return stringVal == "Исправен";
        }

        #endregion
    }
}
