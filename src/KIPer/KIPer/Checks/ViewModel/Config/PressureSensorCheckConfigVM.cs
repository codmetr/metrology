using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Checks.ViewModel.Config
{
    public class PressureSensorCheckConfigVm:INotifyPropertyChanged
    {
        /// <summary>
        /// Принадлежит:
        /// </summary>
        public string Master { get; set; }

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Тип:
        /// </summary>
        public string SensorType { get; set; }

        /// <summary>
        /// Модификация:
        /// </summary>
        public string SensorModel { get; set; }

        /// <summary>
        /// Регистрационный номер в Федеральном информационном фонде по обеспечению единства измерений:
        /// </summary>
        public string RegNum { get; set; }

        /// <summary>
        /// Серия и номер знака предыдущей проверки (если такие серия и номер имеются):
        /// </summary>
        public string NumberLastCheck { get; set; }

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Поверено:
        /// </summary>
        /// <remarks>
        /// Наименование величин, диапазонов, на которых поверено средство измерений (если предусмотрено методикой поверки)
        /// </remarks>
        public string CheckedParameters { get; set; }

        /// <summary>
        /// Поверено в соответствии с:
        /// </summary>
        /// <remarks>
        /// Наименование документа, на основании которого выполнена поверка
        /// </remarks>
        public string ChecklLawBase { get; set; }

        /// <summary>
        /// Эталон давления
        /// </summary>
        public EthalonDescriptor EthalonPressure { get; set; }

        /// <summary>
        /// Эталон напряжения
        /// </summary>
        public EthalonDescriptor EthalonVoltage { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Описатель эталона
    /// </summary>
    public class EthalonDescriptor: INotifyPropertyChanged
    {

        /// <summary>
        /// Наименование:
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Тип:
        /// </summary>
        public string SensorType { get; set; }

        /// <summary>
        /// Заводской номер (номера):
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Регистрационный номер:
        /// </summary>
        public string RegNum { get; set; }

        /// <summary>
        /// Разряд:
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Класс или погрешность:
        /// </summary>
        public string ErrorClass { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
