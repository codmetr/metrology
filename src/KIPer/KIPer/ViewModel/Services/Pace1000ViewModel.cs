using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using PACESeries;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class Pace1000ViewModel : ViewModelBase, IService
    {
        private IDeviceManager _deviceManager;
        private PACE1000Model _model;
        private string _pressure;
        private bool _isAutoRead;
        private TimeSpan _autoreadPeriod;
        private ObservableCollection<ParameterValuePair> _deviceParameters = new ObservableCollection<ParameterValuePair>();
        private string _unit;
        private IEnumerable<PressureUnitDescriptor> _avalableUnits;
        private PressureUnitDescriptor _selectedUnit;

        /// <summary>
        /// Initializes a new instance of the Pace1000ViewModel class.
        /// </summary>
        public Pace1000ViewModel(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            _avalableUnits = new[]
            {
                new PressureUnitDescriptor()
                {
                    Unit = PressureUnits.MBar,
                    UnitString = _pressureUnitToString(PressureUnits.MBar)
                },
                new PressureUnitDescriptor()
                {
                    Unit = PressureUnits.mmHg,
                    UnitString = _pressureUnitToString(PressureUnits.mmHg)
                },
                new PressureUnitDescriptor()
                {
                    Unit = PressureUnits.KgCm2,
                    UnitString = _pressureUnitToString(PressureUnits.KgCm2)
                },
            };
            SelectedUnit = _avalableUnits.First();
        }

        void _model_PressureUnitChanged(object sender, EventArgs e)
        {
            Unit = _pressureUnitToString(_model.PressureUnit);
        }

        void _model_PressureChanged(object sender, EventArgs e)
        {
            Pressure = _model.Pressure.ToString("F3");
        }

        public string Title { get { return _model.Title; } }
        public void Start(int address, ITransportChannelType channel)
        {
            _model = _deviceManager.GetDevice<PACE1000Model>(address, channel);
            _model.PressureChanged += _model_PressureChanged;
            _model.PressureUnitChanged += _model_PressureUnitChanged;
            AutoreadPeriod = _model.AutoreadPeriod;
        }

        public void Stop()
        {
            _model.PressureChanged -= _model_PressureChanged;
            _model.PressureUnitChanged -= _model_PressureUnitChanged;
            _model = null;
        }

        public string Pressure
        {
            get { return _pressure; }
            private set
            {
                Set(ref _pressure, value);
            }
        }

        public string Unit
        {
            get { return _unit; }
            private set { Set(ref _unit, value); }
        }

        public IEnumerable<PressureUnitDescriptor> AvalableUnits
        {
            get { return _avalableUnits; }
        }

        public PressureUnitDescriptor SelectedUnit
        {
            get { return _selectedUnit; }
            set { Set(ref _selectedUnit, value); }
        }

        public ICommand UpdatePressureAndUnits { get { return new CommandWrapper(_updatePressureAndUnit); } }
        public ICommand SetSelectedUnit { get { return new CommandWrapper(()=>_setUnit(_selectedUnit.Unit)); } }

        public bool IsAutoRead
        {
            get { return _isAutoRead; }
            set
            {
                if(value == _isAutoRead)
                    return;
                Set(ref _isAutoRead, value);
                if (_isAutoRead)
                {
                    _model.StartAutoread(_autoreadPeriod);
                }
                else
                {
                    _model.StopAutoUpdate();
                }
            }
        }

        public TimeSpan AutoreadPeriod
        {
            get { return _autoreadPeriod; }
            set
            {
                if (value == _autoreadPeriod)
                    return;
                Set(ref _autoreadPeriod, value);
                _model.SetAutoreadPeriod(_autoreadPeriod);
            }
        }

        private void _setUnit(PressureUnits unit)
        {
            _model.SetPressureUnit(unit);
        }

        private void _updatePressureAndUnit()
        {
            _model.UpdatePressure();
        }

        private string _pressureUnitToString(PressureUnits unit)
        {
            switch (unit)
            {
                case PressureUnits.None:
                    break;
                case PressureUnits.MBar:
                    return "мБар";
                case PressureUnits.inH2O4:
                    break;
                case PressureUnits.inH2O20:
                    break;
                case PressureUnits.inHg:
                    break;
                case PressureUnits.mmHg:
                    return "мм рт.ст.";
                case PressureUnits.Pa:
                    break;
                case PressureUnits.hPa:
                    break;
                case PressureUnits.psi:
                    break;
                case PressureUnits.inH2O60F:
                    break;
                case PressureUnits.KgCm2:
                    return "кгс//см";
                case PressureUnits.FS:
                    break;
                case PressureUnits.mmH2O4:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unit");
            }
            return string.Empty;
        }

        public override void Cleanup()
        {
            if (_model != null)
            {
                _model.PressureChanged -= _model_PressureChanged;
                _model.PressureUnitChanged -= _model_PressureUnitChanged;
            }
            base.Cleanup();
        }
    }
}