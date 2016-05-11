using System;
using ADTS;
using GalaSoft.MvvmLight;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Services
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

        /// <summary>
        /// Initializes a new instance of the ADTSViewModel class.
        /// </summary>
        public ADTSViewModel(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public void Start(ADTSModel model)
        {
            if (_model != null)
            {
                _model.StopAutoUpdate();
                DetachEvents(_model);
            }
            _model = model;
            if (_model != null)
            {
                _model.StartAutoUpdate();
                AttachEvents(_model);
            }
        }

        public string Title { get { return ADTSModel.Model; } }

        public void Start(ITransportChannelType channel)
        {
            var model = _deviceManager.GetDevice<ADTSModel>(channel);
            Start(model);
        }

        public void Stop()
        {
            if (_model != null)
            {
                _model.StopAutoUpdate();
                DetachEvents(_model);
            }
            _model = null;
        }

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

        public string Pressure
        {
            get { return _pressure; }
            private set { Set(ref _pressure, value); }
        }

        public string Pitot
        {
            get { return _pitot; }
            private set { Set(ref _pitot, value); }
        }

        

        #region Events
        private void AttachEvents(ADTSModel model)
        {
            model.PressureReaded += model_PressureReaded;
            model.PitotReaded += _model_PitotReaded;
            model.StateReaded += _model_StateReaded;
            model.StatusReaded += _model_StatusReaded;
        }

        private void DetachEvents(ADTSModel model)
        {
            model.PressureReaded += model_PressureReaded;
            model.PitotReaded += _model_PitotReaded;
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
                Pitot = string.Format("{0}{1}", _model.Pitot,
                    _model.PUnits == null ? "" : PUnitToString(_model.PUnits.Value));
            }
        }

        void model_PressureReaded(System.DateTime obj)
        {
            if (_model != null && _model.Pressure != null)
            {
                Pressure = string.Format("{0}{1}", _model.Pressure,
                    _model.PUnits == null ? "" : PUnitToString(_model.PUnits.Value));
            }
        }

        string PUnitToString(PressureUnits unit)
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