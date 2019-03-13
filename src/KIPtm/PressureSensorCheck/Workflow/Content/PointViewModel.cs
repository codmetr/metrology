using System.ComponentModel;
using System.Runtime.CompilerServices;
using KipTM.Interfaces;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Описатель одной точки проверки
    /// </summary>
    public class PointViewModel : INotifyPropertyChanged
    {
        private IContext _context;
        private int _index;
        private PressureSensorPointConf _config;
        private PointResultViewModel _result;

        /// <summary>
        /// Описатель одной точки проверки
        /// </summary>
        /// <param name="context"></param>
        public PointViewModel(IContext context)
        {
            _context = context;
            _result = new PointResultViewModel();
        }

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

        public PressureSensorPointConf Config
        {
            get { return _config; }
            private set { _config = value;
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

        public void UpdateConf(PressureSensorPointConf config)
        {
            _context.Invoke(()=>Config = config);
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