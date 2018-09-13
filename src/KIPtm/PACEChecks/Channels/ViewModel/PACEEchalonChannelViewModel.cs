using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PACEChecks.Channels.ViewModel
{
    public class PaceEthalonChannelViewModel : INotifyPropertyChanged
    {
        private PaceEtalonChannel _model;
        private bool _isActive;
        private string _pressure;
        private string _pressureUnit;

        /// <summary>
        /// Initializes a new instance of the PACEEchalonChannelViewModel class.
        /// </summary>
        public PaceEthalonChannelViewModel(PaceEtalonChannel model)
        {
            _model = model;
            _model.ActiveStateChange += _model_ActiveStateChange;
            _model.StateUpdated += _model_StateUpdated;
        }

        /// <summary>
        /// Активность канала
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Pressure
        {
            get { return _pressure; }
            set { _pressure = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PressureUnit
        {
            get { return _pressureUnit; }
            set
            {
                _pressureUnit = value;
                OnPropertyChanged();
            }
        }

        void _model_StateUpdated(object sender, System.EventArgs e)
        {
            Pressure = _model.Pressure.ToString("F2");
            PressureUnit = _model.PressureUnit;
        }

        void _model_ActiveStateChange(object sender, System.EventArgs e)
        {
            IsActive = _model.IsActive;
        }

        public virtual void Cleanup()
        {
            _model.ActiveStateChange -= _model_ActiveStateChange;
            _model.StateUpdated -= _model_StateUpdated;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}