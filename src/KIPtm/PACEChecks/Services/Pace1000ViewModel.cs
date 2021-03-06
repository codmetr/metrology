﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Services.ViewModel;
using PACEChecks.Devices;
using PACESeries;
using Tools.View;

namespace PACEChecks.Services
{
    public class Pace1000ViewModel : INotifyPropertyChanged, IService
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
            _model = _deviceManager.GetModel<PACE1000Model>();
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

        public virtual void Cleanup()
        {
            if (_model != null)
            {
                _model.PressureChanged -= _model_PressureChanged;
                _model.PressureUnitChanged -= _model_PressureUnitChanged;
            }
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
               _pressure = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region PressureUnit 

        public string Unit
        {
            get { return _unit; }
            private set { _unit = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<UnitDescriptor<PressureUnits>> AvalableUnits
        {
            get { return _avalableUnits; }
        }

        public UnitDescriptor<PressureUnits> SelectedUnit
        {
            get { return _selectedUnit; }
            set { _selectedUnit = value;
                OnPropertyChanged();
            }
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
                _isAutoRead = value;
                OnPropertyChanged();
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
                _autoreadPeriod = value;
                OnPropertyChanged();
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
                case PressureUnits.Bar:
                    return "бар";
                case PressureUnits.inH2O4:
                    return "дюйм вод.ст. 4С";
                case PressureUnits.inH2O:
                    return "дюйм вод.ст. 20С";
                case PressureUnits.inHg:
                    return "дюйм рт.ст.";
                case PressureUnits.mmHg:
                    return "мм рт.ст.";
                case PressureUnits.Pa:
                    return "Па";
                case PressureUnits.hPa:
                    return "гПа";
                case PressureUnits.kPa:
                    return "кПа";
                case PressureUnits.psi:
                    return "фтс/дюйм";
                case PressureUnits.inH2O60F:
                    return "дюйм вод.ст. 60F";
                case PressureUnits.KgCm2:
                    return "кгс/см";
                case PressureUnits.ATM:
                    return "атм";
                case PressureUnits.mmH2O4:
                    return "мм вод.ст. 4С";
                default:
                    throw new ArgumentOutOfRangeException("unit");
            }
            return string.Empty;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}