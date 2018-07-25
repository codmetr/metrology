using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using ADTS;
using ADTSChecks.Devices;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using Tools.View;

namespace ADTSChecks.ViewModel.Services
{
    public class ADTSViewModel : INotifyPropertyChanged, IService
    {
        private ADTSModel _model;
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
        private IEnumerable<UnitDescriptor<PressureUnits>> _avalableUnits;
        private UnitDescriptor<PressureUnits> _selectedUnit;
        private CancellationTokenSource _cancellation = new CancellationTokenSource();
        private double _pressureAim;
        private double _pitotAim;
        private double _pitotRateAim;
        private double _pressureRateAim;
        private bool _isControlMode = true;

        /// <summary>
        /// Initializes a new instance of the ADTSViewModel class.
        /// </summary>
        public ADTSViewModel(IDeviceManager deviceManager)
        {

            _model = deviceManager.GetModel<ADTSModel>();
            _avalableUnits = new[]
            {
                new UnitDescriptor<PressureUnits>(PressureUnits.MBar, PressureUnits.MBar.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.inH2O4,PressureUnits.inH2O4.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.inHg,PressureUnits.inHg.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.mmHg,PressureUnits.mmHg.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.Pa,PressureUnits.Pa.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.hPa,PressureUnits.hPa.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.psi,PressureUnits.psi.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.inH2O60F,PressureUnits.inH2O60F.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.KgCm2,PressureUnits.KgCm2.ToStr()),
                new UnitDescriptor<PressureUnits>(PressureUnits.mmH2O4,PressureUnits.mmH2O4.ToStr()),
            };
            SelectedUnit = _avalableUnits.First();
        }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get { return ADTSModel.Model; } }

        /// <summary>
        /// Запуск автоопроса
        /// </summary>
        /// <param name="channel"></param>
        public void Start(ITransportChannelType channel)
        {
            _model.StopAutoUpdate();
            DetachEvents(_model);

            _model.Start(channel);
            _model.StartAutoUpdate();
            AttachEvents(_model);
        }

        /// <summary>
        /// Остановить автоопроса
        /// </summary>
        public void Stop()
        {
            if (_model != null)
            {
                _model.StopAutoUpdate();
                DetachEvents(_model);
            }
            _cancellation.Cancel();
            _cancellation = new CancellationTokenSource();
        }

        /// <summary>
        /// Обновить значение давления по каналу Ps
        /// </summary>
        public ICommand UpdatePressure { get { return new CommandWrapper(_uptetePressure); } }

        /// <summary>
        /// Обновить значение давления по каналу Pt
        /// </summary>
        public ICommand UpdatePitot { get { return new CommandWrapper(_updatePitot); } }

        /// <summary>
        /// Обновить диницы измерения давления
        /// </summary>
        public ICommand UpdatePressureUnit { get { return new CommandWrapper(_updatePressureUnit); } }

        /// <summary>
        /// Установить единицы давления
        /// </summary>
        public ICommand SetPressureUnit { get { return new CommandWrapper(() => _setPressureUnit(_selectedUnit.Unit)); } }

        /// <summary>
        /// Перейти в режим измерения
        /// </summary>
        public ICommand SetMeasuring { get { return new CommandWrapper(_setMeasuring); } }

        /// <summary>
        /// Перейти в режим установки давления
        /// </summary>
        public ICommand SetControl { get { return new CommandWrapper(_setControl); } }

        /// <summary>
        /// Стравить давление до атмосферного
        /// </summary>
        public ICommand SetGround { get { return new CommandWrapper(_goToGround); } }

        /// <summary>
        /// Установить целевое давление по каналу Ps
        /// </summary>
        public ICommand SetPressureAim { get { return new CommandWrapper(_setPressureAim); } }

        /// <summary>
        /// Установить целевое давление по каналу Pt
        /// </summary>
        public ICommand SetPitotAim { get { return new CommandWrapper(_setPitotAim); } }

        /// <summary>
        /// Установить скорость установки давления по каналу Ps
        /// </summary>
        public ICommand SetPressureRate { get { return new CommandWrapper(_setPressureRate); } }

        /// <summary>
        /// Установить скорость установки давления по каналу Pt
        /// </summary>
        public ICommand SetPitotRate { get { return new CommandWrapper(_setPitotRate); } }

        /// <summary>
        /// Показать возможности управления оборудованием
        /// </summary>
        public bool IsControlMode
        {
            get { return _isControlMode; }
            set { _isControlMode = value;
                OnPropertyChanged();
            }
        }

        #region State
        /// <summary>
        /// Стабилизированно на значении
        /// </summary>
        public bool IsStableAtAimValue
        {
            get { return _isStableAtAimValue; }
            set { _isStableAtAimValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Стабилизированно на уровне земли
        /// </summary>
        public bool IsSafeAtGround
        {
            get { return _isSafeAtGround; }
            set { _isSafeAtGround = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Наращивает
        /// </summary>
        public bool IsRamping
        {
            get { return _isRamping; }
            set { _isRamping = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// PS установлено
        /// </summary>
        public bool IsPsAtSetPointAndInControlMode
        {
            get { return _isPsAtSetPointAndInControlMode; }
            set { _isPsAtSetPointAndInControlMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// PS устанавливается
        /// </summary>
        public bool IsPsRampingAndAchievingRate
        {
            get { return _isPsRampingAndAchievingRate; }
            set { _isPsRampingAndAchievingRate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// PT установлено
        /// </summary>
        public bool IsPtAtSetPointAndInControlMode
        {
            get { return _isPtAtSetPointAndInControlMode; }
            set { _isPtAtSetPointAndInControlMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// PT устанавливается
        /// </summary>
        public bool IsPtRampingAndAchievingRate
        {
            get { return _isPtRampingAndAchievingRate; }
            set { _isPtRampingAndAchievingRate = value;
                OnPropertyChanged();
            }
        }

        public string StateADTS
        {
            get { return _stateAdts; }
            private set { _stateAdts = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Pressure
        public string Pressure
        {
            get { return _pressure; }
            private set { _pressure = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Pressure Aim

        public double PressureAim
        {
            get { return _pressureAim; }
            set { _pressureAim = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Pressure Rate Aim

        public double PressureRateAim
        {
            get { return _pressureRateAim; }
            set { _pressureRateAim = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Pitot
        public string Pitot
        {
            get { return _pitot; }
            private set { _pitot = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Pitot Aim

        public double PitotAim
        {
            get { return _pitotAim; }
            set { _pitotAim = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Pitot Rate Aim

        public double PitotRateAim
        {
            get { return _pitotRateAim; }
            set { _pitotRateAim = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Pressure Unit
        public IEnumerable<UnitDescriptor<PressureUnits>> AvalableUnits
        {
            get { return _avalableUnits; }
        }

        public UnitDescriptor<PressureUnits> SelectedUnit
        {
            get { return _selectedUnit; }
            set {  _selectedUnit = value;
                OnPropertyChanged();
            }
        }

        public string PressureUnit
        {
            get { return _pressureUnit; }
            set { _pressureUnit = value;
                OnPropertyChanged();
            }
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
                IsStableAtAimValue = (_model.StatusADTS.Value & Status.StableAtAimValue) > 0;
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
                PressureUnit = _model.PUnits.Value.ToStr();
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
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}