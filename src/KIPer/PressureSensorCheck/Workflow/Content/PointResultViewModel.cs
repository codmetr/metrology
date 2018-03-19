using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Результат на точке
    /// </summary>
    public class PointResultViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Фактическое давление
        /// </summary>
        public double PressureReal { get; set; }

        /// <summary>
        /// Фактическое напряжение (прямой ход)
        /// </summary>
        public double IReal { get; set; }

        /// <summary>
        /// Фактическая погрешность (прямой ход)
        /// </summary>
        public double dIReal { get; set; }

        /// <summary>
        /// Фактическое напряжение (обратный ход)
        /// </summary>
        public double Iback { get; set; }

        /// <summary>
        /// Фактическая вариация
        /// </summary>
        public double Ivar { get; set; }

        /// <summary>
        /// Фактическая погрешность вариации
        /// </summary>
        public double dIvar { get; set; }

        /// <summary>
        /// Напряжение на заданной точке в допуске
        /// </summary>
        public bool IsCorrect { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}