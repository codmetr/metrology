using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using KipTM.Interfaces;
using KipTM.Model.Devices;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ADTSViewModel : ViewModelBase, IService
    {
        private readonly ADTSModel _model;
        private double _pressure;
        private double _pitot;
        private bool _isAutoRead;
        private TimeSpan _autoreadPeriod;
        private ObservableCollection<ParameterValuePair> _deviceParameters = new ObservableCollection<ParameterValuePair>();

        /// <summary>
        /// Initializes a new instance of the Pace5000ViewModel class.
        /// </summary>
        public ADTSViewModel(ADTSModel model)
        {
            _model = model;
            _model.PitotReaded += _model_PitotReaded;
            _model.PressureReaded += _model_PressureReaded;
        }

        void _model_PressureReaded(DateTime obj)
        {
            if (_model.Pressure != null) Pressure = _model.Pressure.Value;
        }

        void _model_PitotReaded(DateTime obj)
        {
            if (_model.Pitot != null) Pitot = _model.Pitot.Value;
        }

        public string Title { get { return _model.Title; } }

        public double Pressure
        {
            get { return _pressure; }
            private set
            {
                Set(ref _pressure, value);
            }
        }

        public double Pitot
        {
            get { return _pitot; }
            private set
            {
                Set(ref _pitot, value);
            }
        }

        public bool IsAutoRead
        {
            get { return _isAutoRead; }
            set { Set(ref _isAutoRead, value); }
        }

        public TimeSpan AutoreadPeriod
        {
            get { return _autoreadPeriod; }
            set { Set(ref _autoreadPeriod, value); }
        }

        public ObservableCollection<ParameterValuePair> DeviceParameters
        {
            get { return _deviceParameters; }
            private set { Set(ref _deviceParameters, value); }
        }

        private bool _updateState = false;
        public string NameStartStopUpdateButton{
            get
            {
                return _updateState ? "Старт автообновления" : "Стоп автообновления";
            }
        }

        public ICommand StopAutoUpdate
        {
            get
            {
                return new RelayCommand<object>((param) =>
                {
                    if(_updateState)
                        _model.StopAutoUpdate();
                    else
                        _model.StartAutoUpdate();
                    _updateState = !_updateState;
                    RaisePropertyChanged("NameStartStopUpdateButton");
                });
            }
        }



    }
}