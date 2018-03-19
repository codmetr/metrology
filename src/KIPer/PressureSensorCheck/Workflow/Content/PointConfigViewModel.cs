using System.ComponentModel;
using System.Runtime.CompilerServices;
using KipTM.Interfaces;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Конфигурация точки
    /// </summary>
    public class PointConfigViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Проверяемая точка давления
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// Единицы измерения давления
        /// </summary>
        public Units Unit { get; set; }

        /// <summary>
        /// Ожидаемое значение тока
        /// </summary>
        public double I { get; set; }

        /// <summary>
        /// Допуск по току
        /// </summary>
        public double dI { get; set; }

        /// <summary>
        /// Допуск по вариации напряжения
        /// </summary>
        public double Ivar { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}