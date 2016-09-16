using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using ADTS;
using ADTSChecks.Model.Devices;
using GalaSoft.MvvmLight;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel;

namespace ADTSChecks.ViewModel.Services
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ADTSViewModel : ViewModelBase, IService
    {
        private IDeviceManager _deviceManager;
        private ADTSModel _model;
        private string _statusAdts;
        private string _stateAdts;
        private string _pressure;
        private string _pitot;
        private bool _isStableAtAimValue;
        private bool _isSafeAtGround;
        private bool _isRamping;
        private bool _isPsAtSetPointAndInControlMode;
        private bool _isPsRampingAndAchievingRate;
        private bool _isPtAtSetPointAndInControlMode;
        private bool _isPtRampingAndAchievingRate;
        private string _pressureUnit;
        private IEnumerable<PressureUnitDescriptor<PressureUnits>> _avalableUnits;
        private PressureUnitDescriptor<PressureUnits> _selectedUnit;
        private CancellationTokenSource _cancellation = new CancellationTokenSource();
        private double _pressureAim;
        private double _pitotAim;
        private double _pitotRateAim;
        private double _pressureRateAim;

        /// <summary>
        /// Initializes a new instance of the ADTSViewModel class.
        /// </summary>
        public ADTSViewModel(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            _avalableUnits = new[]
            {
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.MBar,_pressureUnitToString(PressureUnits.MBar)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.inH2O4,_pressureUnitToString(PressureUnits.inH2O4)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.inHg,_pressureUnitToString(PressureUnits.inHg)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.mmHg,_pressureUnitToString(PressureUnits.mmHg)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.Pa,_pressureUnitToString(PressureUnits.Pa)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.hPa,_pressureUnitToString(PressureUnits.hPa)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.psi,_pressureUnitToString(PressureUnits.psi)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.inH2O60F,_pressureUnitToString(PressureUnits.inH2O60F)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.KgCm2,_pressureUnitToString(PressureUnits.KgCm2)),
                new PressureUnitDescriptor<PressureUnits>(PressureUnits.mmH2O4,_pressureUnitToString(PressureUnits.mmH2O4)),
            };
            SelectedUnit = _avalableUnits.First();
        }

        public string Title { get { return ADTSModel.Model; } }

        public void Start(ADTSModel model, ITransportChannelType channel)
        {
            if (_model != null)
            {
                _model.StopAutoUpdate();
                DetachEvents(_model);
            }
            _model = model;
            if (_model != null)
            {
                _model.Start(channel);
                _model.StartAutoUpdate();
                AttachEvents(_model);
            }
        }

        public void Start(ITransportChannelType channel)
        {
            Start(_deviceManager.GetModel<ADTSModel>(), channel);
        }

        public void Stop()
        {
            if (_model != null)
            {
                _model.StopAutoUpdate();
                DetachEvents(_model);
            }
            _cancellation.Cancel();
            _cancellation = new CancellationTokenSource();
            _model = null;
        }

        public ICommand UpdatePressure { get { return new CommandWrapper(_uptetePressure); } }

        public ICommand UpdatePitot { get { return new CommandWrapper(_updatePitot); } }

        public ICommand UpdatePressureUnit { get { return new CommandWrapper(_updatePressureUnit); } }

        public ICommand SetPressureUnit { get { return new CommandWrapper(() => _setPressureUnit(_selectedUnit.Unit)); } }

        public ICommand SetMeasuring { get { return new CommandWrapper(_setMeasuring); } }

        public ICommand SetControl { get { return new CommandWrapper(_setControl); } }

        public ICommand SetGround { get { return new CommandWrapper(_goToGround); } }

        public ICommand SetPressureAim { get { return new CommandWrapper(_setPressureAim); } }

        public ICommand SetPitotAim { get { return new CommandWrapper(_setPitotAim); } }

        public ICommand SetPressureRate { get { return new CommandWrapper(_setPressureRate); } }

        public ICommand SetPitotRate { get { return new CommandWrapper(_setPitotRate); } }

        
        #region State
        /// <summary>
        /// Стабилизированно на значении
        /// </summary>
        public bool IsStableAtAimValue
        {
            get { return _isStableAtAimValue; }
            set { Set(ref _isStableAtAimValue, value); }
        }

        /// <summary>
        /// Стабилизированно на уровне земли
        /// </summary>
        public bool IsSafeAtGround
        {
            get { return _isSafeAtGround; }
            set { Set(ref _isSafeAtGround, value); }
        }

        /// <summary>
        /// Наращивает
        /// </summary>
        public bool IsRamping
        {
            get { return _isRamping; }
            set { Set(ref _isRamping, value); }
        }

        /// <summary>
        /// PS установлено
        /// </summary>
        public bool IsPsAtSetPointAndInControlMode
        {
            get { return _isPsAtSetPointAndInControlMode; }
            set { Set(ref _isPsAtSetPointAndInControlMode, value); }
        }

        /// <summary>
        /// PS устанавливается
        /// </summary>
        public bool IsPsRampingAndAchievingRate
        {
            get { return _isPsRampingAndAchievingRate; }
            set { Set(ref _isPsRampingAndAchievingRate, value); }
        }

        /// <summary>
        /// PT установлено
        /// </summary>
        public bool IsPtAtSetPointAndInControlMode
        {
            get { return _isPtAtSetPointAndInControlMode; }
            set { Set(ref _isPtAtSetPointAndInControlMode, value); }
        }

        /// <summary>
        /// PT устанавливается
        /// </summary>
        public bool IsPtRampingAndAchievingRate
        {
            get { return _isPtRampingAndAchievingRate; }
            set { Set(ref _isPtRampingAndAchievingRate, value); }
        }

        public string StateADTS
        {
            get { return _stateAdts; }
            private set { Set(ref _stateAdts, value); }
        }
        #endregion

        #region Pressure
        public string Pressure
        {
            get { return _pressure; }
            private set { Set(ref _pressure, value); }
        }
        #endregion

        #region Pressure Aim

        public double PressureAim
        {
            get { return _pressureAim; }
            set { Set(ref _pressureAim, value); }
        }

        #endregion

        #region Pressure Rate Aim

        public double PressureRateAim
        {
            get { return _pressureRateAim; }
            set { Set(ref _pressureRateAim, value); }
        }

        #endregion

        #region Pitot
        public string Pitot
        {
            get { return _pitot; }
            private set { Set(ref _pitot, value); }
        }
        #endregion

        #region Pitot Aim

        public double PitotAim
        {
            get { return _pitotAim; }
            set { Set(ref _pitotAim, value); }
        }

        #endregion

        #region Pitot Rate Aim

        public double PitotRateAim
        {
            get { return _pitotRateAim; }
            set { Set(ref _pitotRateAim, value); }
        }

        #endregion

        #region Pressure Unit
        public IEnumerable<PressureUnitDescriptor<PressureUnits>> AvalableUnits
        {
            get { return _avalableUnits; }
        }

        public PressureUnitDescriptor<PressureUnits> SelectedUnit
        {
            get { return _selectedUnit; }
            set { Set(ref _selectedUnit, value); }
        }

        public string PressureUnit
        {
            get { return _pressureUnit; }
            set { Set(ref _pressureUnit, value); }
        }
        #endregion

        #region Events
        private void AttachEvents(ADTSModel model)
        {
            model.PressureReaded += model_PressureReaded;
            model.PitotReaded += _model_PitotReaded;
            model.PressureUnitChanged += model_PressureUnitChanged;
            model.StateReaded += _model_StateReaded;
            model.StatusReaded += _model_StatusReaded;
        }

        private void DetachEvents(ADTSModel model)
        {
            model.PressureReaded += model_PressureReaded;
            model.PitotReaded += _model_PitotReaded;
            model.PressureUnitChanged -= model_PressureUnitChanged;
            model.StateReaded += _model_StateReaded;
            model.StatusReaded += _model_StatusReaded;
        }

        void _model_StatusReaded(System.DateTime obj)
        {
            if (_model != null && _model.StatusADTS != null)
            {
                IsStableAtAimValue = (_model.StatusADTS.Value & Status.StableAtAimValue)>0;
                IsSafeAtGround = (_model.StatusADTS.Value & Status.SafeAtGround) > 0;
                IsRamping = (_model.StatusADTS.Value & Status.Ramping) > 0;
                IsPsAtSetPointAndInControlMode = (_model.StatusADTS.Value & Status.PsAtSetPointAndInControlMode) > 0;
                IsPsRampingAndAchievingRate = (_model.StatusADTS.Value & Status.PsRampingAndAchievingRate) > 0;
                IsPtAtSetPointAndInControlMode = (_model.StatusADTS.Value & Status.PtAtSetPointAndInControlMode) > 0;
                IsPtRampingAndAchievingRate = (_model.StatusADTS.Value & Status.PtRampingAndAchievingRate) > 0;
            }
        }

        void _model_StateReaded(System.DateTime obj)
        {
            if (_model != null && _model.StateADTS != null)
            {
                switch (_model.StateADTS.Value)
                {
                    case State.None:
                        StateADTS = "";
                        break;
                    case State.Control:
                        StateADTS = "Контроль";
                        break;
                    case State.Measure:
                        StateADTS = "Измерение";
                        break;
                    case State.Hold:
                        StateADTS = "Остановка";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        void _model_PitotReaded(System.DateTime obj)
        {
            if (_model != null && _model.Pitot != null)
            {
                Pitot = _model.Pitot.Value.ToString("F4");
            }
        }

        void model_PressureReaded(System.DateTime obj)
        {
            if (_model != null && _model.Pressure != null)
            {
                Pressure = _model.Pressure.Value.ToString("F4");
            }
        }

        void model_PressureUnitChanged(DateTime obj)
        {
            if (_model != null && _model.PUnits != null)
            {
                PressureUnit = _pressureUnitToString(_model.PUnits.Value);
            }
        }
        #endregion

        #region Services

        private void _uptetePressure()
        {
            _model.UpdatePressure(_cancellation.Token);
        }

        private void _updatePitot()
        {
            _model.UpdatePitot(_cancellation.Token);
        }

        private void _updatePressureUnit()
        {
            _model.UpdatePressureUnit(_cancellation.Token);
        }

        private void _setPressureUnit(PressureUnits unit)
        {
            _model.SetPressureUnit(unit, _cancellation.Token);
        }

        private void _setControl()
        {
            _model.SetState(State.Control, _cancellation.Token);
        }

        private void _setMeasuring()
        {
            _model.SetState(State.Measure, _cancellation.Token);
        }

        private void _goToGround()
        {
            _model.GoToGround(_cancellation.Token);
        }

        private void _setPressureAim()
        {
            _model.SetParameter(Parameters.PS, PressureAim, _cancellation.Token);
        }

        private void _setPitotAim()
        {
            _model.SetParameter(Parameters.PT, PitotAim, _cancellation.Token);
        }

        private void _setPressureRate()
        {
            _model.SetRate(Parameters.PS, PressureRateAim, _cancellation.Token);
        }

        private void _setPitotRate()
        {
            _model.SetRate(Parameters.PT, PitotRateAim, _cancellation.Token);
        }

        string _pressureUnitToString(PressureUnits unit)
        {
            switch (unit)
            {
                case PressureUnits.None:
                    break;
                case PressureUnits.MBar:
                    return "мБар";
                case PressureUnits.inH2O4:
                    return "дюйм.вод.ст.";
                case PressureUnits.inH2O20:
                    return "дюйм.вод.ст.20С";
                case PressureUnits.inHg:
                    return "дюйм.рт.ст.";
                case PressureUnits.mmHg:
                    return "мм рт.ст";
                case PressureUnits.Pa:
                    return "Па";
                case PressureUnits.hPa:
                    return "гПа";
                case PressureUnits.psi:
                    return "Фунт-сила на кв. дюйм";
                case PressureUnits.inH2O60F:
                    return "дюйм.вод.ст.60F";
                case PressureUnits.KgCm2:
                    return "кгс//см";
                case PressureUnits.FS:
                    return "Фунт-сила";
                case PressureUnits.mmH2O4:
                    return "мм вод.ст";
                default:
                    throw new ArgumentOutOfRangeException("unit");
            }
            return string.Empty;
        }
        #endregion
    }
}