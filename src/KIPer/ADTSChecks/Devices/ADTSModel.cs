using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using ADTSData;
using ArchiveData.DTO;
using KipTM.Model;
using KipTM.Model.TransportChannels;
using MainLoop;

namespace ADTSChecks.Devices
{
    /// <summary>
    /// Модель ADTS-450
    /// </summary>
    public class ADTSModel
    {
        /// <summary>
        /// Описатель типа ADTS450
        /// </summary>
        public static DeviceTypeDescriptor Descriptor =>
             new DeviceTypeDescriptor(ADTSModel.Model, ADTSModel.DeviceCommonType, ADTSModel.DeviceManufacturer)
             {
                 TypeKey = ADTSModel.Key,
                 Function = DeviceTypeDescriptor.FunctionType.Controller,
             };

        public static string Key { get { return "ADTS"; } }
        public static string Model { get { return KeysDic.ADTSModelKey; } }
        public static string DeviceCommonType { get { return "Калибратор давления"; } }
        public static string DeviceManufacturer { get { return "GE Druk"; } }
        public static IEnumerable<string> TypesEtalonParameters = new[]
            {"давление", "авиационная высота", "авиационная скорость"};

        /// <summary>
        /// Маркер канала Ps
        /// </summary>
        public const string Ps = "ADTS.Ps";
        /// <summary>
        /// Маркер канала Pt
        /// </summary>
        public const string Pt = "ADTS.Pt";

        #region Local members

        private IDeviceManager _deviceManager;
        private ADTSDriver _adts;
        private readonly ILoops _loops;
        private string _loopKey = null;
        private bool _isNeedAutoupdate;


        private TimeSpan _periodUpdatePressure = TimeSpan.FromMilliseconds(100);
        private TimeSpan _periodUpdatePitot = TimeSpan.FromMilliseconds(100);
        private TimeSpan _periodUpdateState = TimeSpan.FromMilliseconds(100);
        private TimeSpan _periodUpdateStatus = TimeSpan.FromMilliseconds(100);

        private DateTime? _pressureTime;
        private double? _pressure;

        private DateTime? _pitotTime;
        private double? _pitot;

        private PressureUnits? _pressureUnit;

        private DateTime? _stateTime;
        private State? _state;

        private DateTime? _statusTime;
        private Status? _status;

        private IDictionary<EventWaitHandle, Func<Status, bool>> _waitStatusPool; 
        private IDictionary<EventWaitHandle, Func<State, bool>> _waitStatePool; 
        #endregion

        /// <summary>
        /// Модель ADTS430
        /// </summary>
        public ADTSModel(string title, ILoops loops, IDeviceManager deviceManager)
        {
            Title = title;
            _loops = loops;
            _deviceManager = deviceManager;
            _waitStatusPool = new Dictionary<EventWaitHandle, Func<Status, bool>>();
            _waitStatePool = new Dictionary<EventWaitHandle, Func<State, bool>>();
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        public void Start(ITransportChannelType transport)
        {
            _loopKey = transport.Key;
            _adts = _deviceManager.GetDevice<ADTSDriver>(transport);
            _loops.StartUnimportantAction(_loopKey, _updateUnit);
        }

        /// <summary>
        /// Запуск автоопроса модуля дискретных входов
        /// </summary>
        public void StartAutoUpdate()
        {
            _isNeedAutoupdate = true;
            _loops.StartUnimportantAction(_loopKey, AutoUpdateStatus);
            _loops.StartUnimportantAction(_loopKey, AutoUpdateState);
            _loops.StartUnimportantAction(_loopKey, AutoUpdatePressure);
            _loops.StartUnimportantAction(_loopKey, AutoUpdatePitot);
        }
        
        /// <summary>
        /// Остановка автоопроса модуля дискретных входов
        /// </summary>
        public void StopAutoUpdate()
        {
            _isNeedAutoupdate = false;
        }

        /// <summary>
        /// Установить состояние
        /// </summary>
        /// <param name="state"></param>
        public bool SetState(State state, CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.SetState(state) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                result = true;
                isCommpete.Set();
            });
            if (cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if (cancel.IsCancellationRequested)
                return false;
            if (result)
                _state = state;
            return result;
        }

        /// <summary>
        /// Установить состояние
        /// </summary>
        public bool GoToGround(CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.GoToGround() || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                result = true;
                isCommpete.Set();
            });
            if (cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if (cancel.IsCancellationRequested)
                return false;
            return result;
        }

        #region Calibration
        /// <summary>
        /// Запуск процесса калибровки
        /// </summary>
        /// <param name="channel">Одно из изначений ключей канала: <see cref="ADTSModel.Ps"/> или <see cref="ADTSModel.Pt"/></param>
        /// <param name="date"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool StartCalibration(string channel, out DateTime? date, CancellationToken cancel)
        {
            bool result = false;
            date = null;
            var isCommpete = new ManualResetEvent(false);
            DateTime? dateValue = null;
            if (cancel.IsCancellationRequested)
                return false;
            var calibChannel = CalibChannel.PS;
            if (channel == ADTSModel.Pt)
                calibChannel = CalibChannel.PT;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.GetDate(out dateValue) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.SetState(State.Control) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.StartCalibration(calibChannel) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                result = true;
                isCommpete.Set();
            });
            if(cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if(cancel.IsCancellationRequested)
                return false;
            if (result)
                date = dateValue;
            return result;
        }
        #endregion

        #region Pressure Unit
        /// <summary>
        /// Задать единицы измерения давления
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool SetPressureUnit(PressureUnits unit, CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            if(cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.SetUnits(unit) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }
                result = true;
                isCommpete.Set();
                PUnits = unit;
            });
            if(cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if(cancel.IsCancellationRequested)
                return false;
            return result;
        }

        public bool UpdatePressureUnit(CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            if (cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                PressureUnits? unit;
                if (!_adts.GetUnits(out unit) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }
                result = true;
                isCommpete.Set();
                PUnits = unit;
            });
            if (cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if (cancel.IsCancellationRequested)
                return false;
            return result;
        }

        #endregion

        #region Rate

        /// <summary>
        /// Задать скорость установки величины
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="rate"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool SetRate(Parameters parameter, double rate, CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            if(cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.SetRate(parameter, rate) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }
                result = true;
                isCommpete.Set();
            });
            if(cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if(cancel.IsCancellationRequested)
                return false;
            return result;
        }

        #endregion

        #region Any parameter
        /// <summary>
        /// Задать цель для выбранного параметра
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="aim"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool SetParameter(Parameters parameter, double aim, CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            if(cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.SetAim(parameter, aim) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }
                result = true;
                isCommpete.Set();
            });
            if(cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if(cancel.IsCancellationRequested)
                return false;
            return result;
        }
        #endregion

        #region Pressure

        /// <summary>
        /// Давление
        /// </summary>
        public double? Pressure
        {
            get { return _pressure; }
            private set
            {
                if (value == null)
                    return;
                _pressure = value;
                _pressureTime = DateTime.Now;
                OnPressureReaded(_pressureTime.Value);
            }
        }

        /// <summary>
        /// Обновить значение давления PS
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool UpdatePressure(CancellationToken cancel)
        {
            return _userUpdater(cancel, (t) => _updater(t, (val) => Pressure = val, Parameters.PS));
        }

        void AutoUpdatePressure(object transport)
        {
            _autoUpdater(transport, () => _periodUpdatePressure, t=>_updater(t, (val)=>Pressure=val, Parameters.PS));
        }
        #endregion

        #region Pitot
        /// <summary>
        /// Полное давление 
        /// </summary>
        public double? Pitot
        {
            get { return _pitot; }
            private set
            {
                if (value == null)
                    return;
                _pitot = value;
                _pitotTime = DateTime.Now;
                OnPitotReaded(_pitotTime.Value);
            }
        }

        /// <summary>
        /// Обновить значение давления PT
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool UpdatePitot(CancellationToken cancel)
        {
            return _userUpdater(cancel, (t) => _updater(t, val => Pitot = val, Parameters.PT));
        }

        void AutoUpdatePitot(object transport)
        {
            _autoUpdater(transport, () => _periodUpdatePitot, t => _updater(t, (val) => Pitot = val, Parameters.PT));
        }
        #endregion

        #region PUnit

        public PressureUnits? PUnits
        {
            get
            {
                return _pressureUnit;
            }
            private set
            {
                if (value == _pressureUnit)
                    return;
                _pressureUnit = value;
                OnPressureUnitChanged(DateTime.Now);
            }
        }

        /// <summary>
        /// Обновить значение давления PT
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool UpdateUnit(CancellationToken cancel)
        {
            return _userUpdater(cancel, _updateUnit);
        }

        void _updateUnit(object transport)
        {
            PressureUnits? unit;
            if (!_adts.GetUnits(out unit))
                return;
            PUnits = unit;
        }
        #endregion

        /// <summary>
        /// Ожидать достяжения Ps необходимого давления
        /// </summary>
        /// <returns></returns>
        public EventWaitHandle WaitPressureSetted()
        {
            return WaitStatusADTS(status => (status & Status.PsAtSetPointAndInControlMode) > 0);
        }

        /// <summary>
        /// Ожидать достяжения Pt необходимого давления
        /// </summary>
        /// <returns></returns>
        public EventWaitHandle WaitPitotSetted()
        {
            return WaitStatusADTS(status => (status & Status.PtAtSetPointAndInControlMode) > 0);
        }

        /// <summary>
        /// Ожидать достяжения режима Контроль
        /// </summary>
        /// <returns></returns>
        public EventWaitHandle WaitControlSetted()
        {
            return WaitStateADTS(state => state == State.Control);
        }

        /// <summary>
        /// Перестат ожидать
        /// </summary>
        /// <param name="waitHandle"></param>
        public void StopWaitStatus(EventWaitHandle waitHandle)
        {
            lock (_waitStatusPool)
            {
                if (_waitStatusPool.ContainsKey(waitHandle))
                    _waitStatusPool.Remove(waitHandle);
            }
        }

        /// <summary>
        /// Перестат ожидать
        /// </summary>
        /// <param name="waitHandle"></param>
        public void StopWaitState(EventWaitHandle waitHandle)
        {
            lock (_waitStatePool)
            {
                if (_waitStatePool.ContainsKey(waitHandle))
                    _waitStatePool.Remove(waitHandle);
            }
        }

        /// <summary>
        /// Задать действительное значение давления
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool SetActualValue(double value, CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            if (cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.SetCalibrationValue(value) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }
                result = true;
                isCommpete.Set();
            });
            if (cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if (cancel.IsCancellationRequested)
                return false;
            return result;
        }

        /// <summary>
        /// Получить результат калибровки
        /// </summary>
        /// <param name="slope"></param>
        /// <param name="zero"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool GetCalibrationResult(out double? slope, out double? zero, CancellationToken cancel)
        {
            bool result = false;
            slope = null;
            zero = null;
            var isCommpete = new ManualResetEvent(false);

            if (cancel.IsCancellationRequested)
                return false;

            double? slopeValue = null;
            double? zeroValue = null;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.GetCamibrationResult(out slopeValue, out zeroValue) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }
                result = true;
                isCommpete.Set();
            });
            if (cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if (cancel.IsCancellationRequested)
                return false;
            if (result)
            {
                slope = slopeValue;
                zero = zeroValue;
            }
            return result;

        }

        /// <summary>
        /// Подтвердить или отменить калибровку
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool AcceptCalibration(bool accept, CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            if (cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                if (!_adts.SetCalibrationAccept(accept) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }
                result = true;
                isCommpete.Set();
            });
            if (cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if (cancel.IsCancellationRequested)
                return false;
            return result;
        }

        public string Title {get; private set;}
        
        public Status? StatusADTS{get { return _status; }}

        public State? StateADTS { get { return _state; } }

        #region Events
        public event Action<DateTime> StatusReaded;

        public event Action<DateTime> StateReaded;

        public event Action<DateTime> PressureReaded;

        public event Action<DateTime> PitotReaded;

        public event Action<DateTime> PressureUnitChanged;

        #region Event invocators

        protected virtual void OnStatusReaded(DateTime obj)
        {
            Action<DateTime> handler = StatusReaded;
            if (handler != null) handler(obj);
        }

        protected virtual void OnStateReaded(DateTime obj)
        {
            Action<DateTime> handler = StateReaded;
            if (handler != null) handler(obj);
        }

        protected virtual void OnPressureReaded(DateTime obj)
        {
            Action<DateTime> handler = PressureReaded;
            if (handler != null) handler(obj);
        }

        protected virtual void OnPitotReaded(DateTime obj)
        {
            Action<DateTime> handler = PitotReaded;
            if (handler != null) handler(obj);
        }

        protected virtual void OnPressureUnitChanged(DateTime obj)
        {
            Action<DateTime> handler = PressureUnitChanged;
            if (handler != null) handler(obj);
        }
        #endregion

        #endregion

        #region Service members

        /// <summary>
        /// Ожидать статус ADTS
        /// </summary>
        /// <param name="waitFunc">Функция проверки статуса</param>
        /// <returns></returns>
        private EventWaitHandle WaitStatusADTS(Func<Status, bool> waitFunc)
        {
            if (waitFunc == null)
                return null;
            var wh = new ManualResetEvent(false);
            lock (_waitStatusPool)
            {
                _waitStatusPool.Add(wh, waitFunc);
            }
            return wh;
        }

        /// <summary>
        /// Ожидать состояние ADTS
        /// </summary>
        /// <param name="waitFunc">Функция проверки состояния</param>
        /// <returns></returns>
        private EventWaitHandle WaitStateADTS(Func<State, bool> waitFunc)
        {
            if (waitFunc == null)
                return null;
            var wh = new ManualResetEvent(false);
            lock (_waitStatePool)
            {
                _waitStatePool.Add(wh, waitFunc);
            }
            return wh;
        }

        #region Autoupdate functions

        void AutoUpdateStatus(object transport)
        {
            if (!_isNeedAutoupdate)
                return;
            if (!_adts.GetStatus(out _status))
                return;
            if(_status == null)
                return;
            _statusTime = DateTime.Now;
            OnStatusReaded(_statusTime.Value);

            lock (_waitStatusPool)
            {
                if (_waitStatusPool.Count > 0)
                {
                    var completeList = (from func in _waitStatusPool where func.Value(_status.Value) select func.Key).ToList();
                    foreach (var waitHandle in completeList)
                    {
                        _waitStatusPool.Remove(waitHandle);
                        waitHandle.Set();
                    }
                }
            }

            if (!_isNeedAutoupdate)
                return;

            Task.Run(() => {
                Thread.Sleep(_periodUpdateStatus);
                _loops.StartUnimportantAction(_loopKey, AutoUpdateStatus);
            });
        }

        void AutoUpdateState(object transport)
        {
             if (!_isNeedAutoupdate)
                return;
            if (!_adts.GetState(out _state))
                return;
            if (_state == null)
                return;
            _stateTime = DateTime.Now;
            OnStateReaded(_stateTime.Value);

            lock (_waitStatePool)
            {
                if (_waitStatePool.Count > 0)
                {
                    var completeList = (from func in _waitStatePool where func.Value(_state.Value) select func.Key).ToList();
                    foreach (var waitHandle in completeList)
                    {
                        _waitStatePool.Remove(waitHandle);
                        waitHandle.Set();
                    }
                }
            }

            if (!_isNeedAutoupdate)
                return;

            Task.Run(() => {
                Thread.Sleep(_periodUpdateState);
                _loops.StartUnimportantAction(_loopKey, AutoUpdateState);
            });
        }

        #endregion

        /// <summary>
        /// Обновление параметра
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="seter"></param>
        /// <param name="parameter"></param>
        void _updater(object transport, Action<double?> seter, Parameters parameter)
        {
            double? paramValue;
            if (!_adts.ReadMeasure(parameter, out paramValue))
                return;
            seter(paramValue);
        }

        /// <summary>
        /// Автообновление параметра
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="getPeriodUpdate"></param>
        /// <param name="updater"></param>
        void _autoUpdater(object transport, Func<TimeSpan> getPeriodUpdate, Action<object> updater)
        {
            if (!_isNeedAutoupdate)
                return;
            updater(transport);

            if (!_isNeedAutoupdate)
                return;

            Task.Run(() =>
            {
                Thread.Sleep(getPeriodUpdate());
                _loops.StartUnimportantAction(_loopKey, t => _autoUpdater(t, getPeriodUpdate, updater));
            });
        }

        /// <summary>
        /// Обновение по запросу пользователя
        /// </summary>
        /// <param name="cancel"></param>
        /// <param name="updater"></param>
        /// <returns></returns>
        public bool _userUpdater(CancellationToken cancel, Action<object> updater)
        {
            bool result = false;
            var isCommpete = new ManualResetEvent(false);
            if (cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (trarsport) =>
            {
                try
                {
                    updater(trarsport);
                }
                finally
                {
                    isCommpete.Set();
                }
            });
            if (cancel.IsCancellationRequested)
                return false;
            isCommpete.WaitOne();
            if (cancel.IsCancellationRequested)
                return false;
            return result;
        }
        #endregion


    }
}
