using GalaSoft.MvvmLight;
using KipTM.Model.Channels;

namespace KipTM.ViewModel.Channels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PACEEchalonChannelViewModel : ViewModelBase
    {
        private PACEEthalonChannel _model;
        private bool _isActive;
        private string _pressure;
        private string _pressureUnit;

        /// <summary>
        /// Initializes a new instance of the PACEEchalonChannelViewModel class.
        /// </summary>
        public PACEEchalonChannelViewModel(PACEEthalonChannel model)
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
            set { Set(ref _isActive, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Pressure
        {
            get { return _pressure; }
            set { Set(ref _pressure, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PressureUnit
        {
            get { return _pressureUnit; }
            set { Set(ref _pressureUnit, value); }
        }

        void _model_StateUpdated(object sender, System.EventArgs e)
        {
            Pressure = _model.Pressure.ToString("F2");
            PressureUnit = _model.PressureUnit;
        }

        void _model_ActiveStateChange(object sender, System.EventArgs e)
        {
            IsActive = _model.Activate();
        }

        public override void Cleanup()
        {
            _model.ActiveStateChange -= _model_ActiveStateChange;
            _model.StateUpdated -= _model_StateUpdated;
            base.Cleanup();
        }
    }
}