using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Описатель одной точки проверки
    /// </summary>
    public class PointViewModel : INotifyPropertyChanged
    {
        private int _index;
        private PointConfigViewModel _config;
        private PointResultViewModel _result;

        /// <summary>
        /// Номер точки
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value;
                OnPropertyChanged();
            }
        }

        public PointConfigViewModel Config
        {
            get { return _config; }
            set { _config = value;
                OnPropertyChanged();
            }
        }

        public PointResultViewModel Result
        {
            get { return _result; }
            set { _result = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}