using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ADTSChecks.Model.Devices;
using CheckFrame.Interfaces;
using CheckFrame.Model;
using CheckFrame.Model.TransportChannels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using PACESeries;
using Tools.View;

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
        private IEnumerable<UnitDescriptor<PressureUnits>> _avalableUnits;
        private UnitDescriptor<PressureUnits> _selectedUnit;

        /// <summary>
        /// Initializes a new instance of the Pace1000ViewModel class.
        /// </summary>
        public Pace1000ViewModel(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            _avalableUnits = new[]
            {
                new UnitDescriptor<PressureUnits>(PressureUnits.MBar,_pressureUnitToString(PressureUnits.MBar)),
                new UnitDescriptor<PressureUnits>(PressureUnits.Bar,_pressureUnitToString(PressureUnits.Bar)),
                new UnitDescriptor<PressureUnits>(PressureUnits.inH2O4,_pressureUnitToString(PressureUnits.inH2O4)),
                new UnitDescriptor<PressureUnits>(PressureUnits.inH2O,_pressureUnitToString(PressureUnits.inH2O)),
                new UnitDescriptor<PressureUnits>(PressureUnits.inHg,_pressureUnitToString(PressureUnits.inHg)),
                new UnitDescriptor<PressureUnits>(PressureUnits.mmHg,_pressureUnitToString(PressureUnits.mmHg)),
                new UnitDescriptor<PressureUnits>(PressureUnits.Pa,_pressureUnitToString(PressureUnits.Pa)),
                new UnitDescriptor<PressureUnits>(PressureUnits.hPa,_pressureUnitToString(PressureUnits.hPa)),
                new UnitDescriptor<PressureUnits>(PressureUnits.kPa,_pressureUnitToString(PressureUnits.kPa)),
                new UnitDescriptor<PressureUnits>(PressureUnits.psi,_pressureUnitToString(PressureUnits.psi)),
                new UnitDescriptor<PressureUnits>(PressureUnits.inH2O60F,_pressureUnitToString(PressureUnits.inH2O60F)),
                new UnitDescriptor<PressureUnits>(PressureUnits.KgCm2,_pressureUnitToString(PressureUnits.KgCm2)),
                new UnitDescriptor<PressureUnits>(PressureUnits.ATM,_pressureUnitToString(PressureUnits.ATM)),
                new UnitDescriptor<PressureUnits>(PressureUnits.mmH2O4,_pressureUnitToString(PressureUnits.mmH2O4)),
            };
            SelectedUnit = _avalableUnits.First();
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

        public string Title { get { return PACE1000Model.Model; } }

        public void Start(ITransportChannelType channel)
        {
            _model.Start(channel);
            _model.PressureChanged += _model_PressureChanged;
            _model.PressureUnitChanged += _model_PressureUnitChanged;
            AutoreadPeriod = _model.AutoreadPeriod;
        }

        public void Stop()
        {
            _model.StopAutoUpdate();
            _model.PressureChanged -= _model_PressureChanged;
            _model.PressureUnitChanged -= _model_PressureUnitChanged;
            _model = null;
        }

        #region Pressure 
        public string Pressure
        {
            get { return _pressure; }
            private set
            {
                Set(ref _pressure, value);
            }
        }
        #endregion

        #region PressureUnit 

        public string Unit
        {
            get { return _unit; }
            private set { Set(ref _unit, value); }
        }

        public IEnumerable<UnitDescriptor<PressureUnits>> AvalableUnits
        {
            get { return _avalableUnits; }
        }

        public UnitDescriptor<PressureUnits> SelectedUnit
        {
            get { return _selectedUnit; }
            set { Set(ref _selectedUnit, value); }
        }
        #endregion

        #region Commands
        public ICommand UpdatePressureAndUnits { get { return new CommandWrapper(_updatePressureAndUnit); } }
        public ICommand UpdateUnits { get { return new CommandWrapper(_updateUnit); } }
        public ICommand SetSelectedUnit { get { return new CommandWrapper(()=>_setUnit(_selectedUnit.Unit)); } }
        public ICommand SetLloOn { get { return new CommandWrapper(()=>_model.SetLloOn()); } }
        public ICommand SetLloOff { get { return new CommandWrapper(()=>_model.SetLloOff()); } }
        public ICommand SetLocal { get { return new CommandWrapper(()=>_model.SetLocal()); } }
        public ICommand SetRemote { get { return new CommandWrapper(()=>_model.SetRemote()); } }
        #endregion

        #region Autoread
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
        #endregion

        #region _Services
        void _model_PressureUnitChanged(object sender, EventArgs e)
        {
            Unit = _pressureUnitToString(_model.PressureUnit);
        }

        void _model_PressureChanged(object sender, EventArgs e)
        {
            Pressure = _model.Pressure.ToString("F3");
        }

        private void _setUnit(PressureUnits unit)
        {
            _model.SetPressureUnit(unit);
        }

        private void _updateUnit()
        {
            _model.UpdateUnit();
            Task.Delay(TimeSpan.FromMilliseconds(100));
            SelectedUnit = AvalableUnits.FirstOrDefault(el=>el.Unit ==_model.PressureUnit);
        }

        private void _updatePressureAndUnit()
        {
            _model.UpdatePressure();
            _model.UpdateUnit();
        }

        private string _pressureUnitToString(PressureUnits unit)
        {
            switch (unit)
            {
                case PressureUnits.None:
                    break;
                case PressureUnits.MBar:
                    return "мБар";
                    break;
                case PressureUnits.Bar:
                    return "бар";
                    break;
                case PressureUnits.inH2O4:
                    return "дюйм вод.ст. 4С";
                    break;
                case PressureUnits.inH2O:
                    return "дюйм вод.ст. 20С";
                    break;
                case PressureUnits.inHg:
                    return "дюйм рт.ст.";
                    break;
                case PressureUnits.mmHg:
                    return "мм рт.ст.";
                    break;
                case PressureUnits.Pa:
                    return "Па";
                    break;
                case PressureUnits.hPa:
                    return "гПа";
                    break;
                case PressureUnits.kPa:
                    return "кПа";
                    break;
                case PressureUnits.psi:
                    return "фтс/дюйм";
                    break;
                case PressureUnits.inH2O60F:
                    return "дюйм вод.ст. 60F";
                    break;
                case PressureUnits.KgCm2:
                    return "кгс/см";
                    break;
                case PressureUnits.ATM:
                    return "атм";
                    break;
                case PressureUnits.mmH2O4:
                    return "мм вод.ст. 4С";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unit");
            }
            return string.Empty;
        }
        #endregion
    }
}