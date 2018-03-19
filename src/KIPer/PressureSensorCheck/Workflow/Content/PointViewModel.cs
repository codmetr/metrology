using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Описатель одной точки проверки
    /// </summary>
    public class PointViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Номер точки
        /// </summary>
        public int Index { get; set; }

        public PointConfigViewModel Config { get; set; }

        public PointResultViewModel Result { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}