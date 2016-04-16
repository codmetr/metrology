using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using IEEE488;
using MainLoop;

namespace KipTM.Model.Devices
{
    public class ADTSModel
    {
        #region Local members
        private readonly ADTSDriverByCommonChannel _adts;
        private readonly ILoops _loops;
        private readonly string _loopKey;
        private bool _isNeedAutoupdate;


        private DateTime? _pressureTime;
        private double? _pressure;

        private DateTime? _pitotTime;
        private double? _pitot;

        private DateTime? _pressureUnitTime;
        private PressureUnits? _pressureUnit;

        private DateTime? _stateTime;
        private State? _state;

        private DateTime? _statusTime;
        private Status? _status;

        private IDictionary<EventWaitHandle, Func<Status, bool>> _waitStatusPool; 
        #endregion

        internal static string Key { get { return "ADTS"; } }
        internal static string Model { get { return "ADTS405"; } }
        internal static string DeviceCommonType { get { return "Калибратор давления"; } }
        internal static string DeviceManufacturer { get { return "GE Druk"; } }
        internal static IEnumerable<string> TypesEtalonParameters = new[]
        {"давление", "авиационная высота", "авиационная скорость"};

        public ADTSModel(string title, ILoops loops, string loopKey, ADTSDriverByCommonChannel driver)
        {
            Title = title;
            _loops = loops;
            _loopKey = loopKey;
            _adts = driver;
            _waitStatusPool = new Dictionary<EventWaitHandle, Func<Status, bool>>();
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        public void Init()
        {
            _loops.StartUnimportantAction(_loopKey, UpdateUnit);
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
        /// Запуск процесса калибровки
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="date"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool StartCalibration(CalibChannel channel, out DateTime? date, CancellationToken cancel)
        {
            bool result = false;
            date = null;
            var isCommpete = new AutoResetEvent(false);
            DateTime? dateValue = null;
            if (cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                var ieee488 = transport as ITransportIEEE488;
                if (ieee488 == null || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.GetDate(ieee488, out dateValue) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.SetState(ieee488, State.Control) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.StartCalibration(ieee488, channel) || cancel.IsCancellationRequested)
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

        /// <summary>
        /// Задать цель для Ps
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="pressure"></param>
        /// <param name="rate"></param>
        /// <param name="unit"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool SetPressure(Parameters parameter, double pressure, double rate, PressureUnits unit, CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new AutoResetEvent(false);
            if(cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                var ieee488 = transport as ITransportIEEE488;
                if (ieee488 == null || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.SetUnits(ieee488, unit) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }
                if (!_adts.SetRate(ieee488, parameter, rate) || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.SetAim(ieee488, parameter, pressure) || cancel.IsCancellationRequested)
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

        /// <summary>
        /// Ожидать достяжения Ps необходимого давления
        /// </summary>
        /// <returns></returns>
        public EventWaitHandle WaitPressureSetted()
        {
            return WaitStatusADTS(status => status == Status.PsAtSetPointAndInControlMode);
        }

        /// <summary>
        /// Ожидать достяжения Pt необходимого давления
        /// </summary>
        /// <returns></returns>
        public EventWaitHandle WaitPitotSetted()
        {
            return WaitStatusADTS(status => status == Status.PtAtSetPointAndInControlMode);
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
        /// Задать действительное значение давления
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public bool SetActualValue(double value, CancellationToken cancel)
        {
            bool result = false;
            var isCommpete = new AutoResetEvent(false);
            if (cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                var ieee488 = transport as ITransportIEEE488;
                if (ieee488 == null || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.SetCalibrationValue(ieee488, value) || cancel.IsCancellationRequested)
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
            var isCommpete = new AutoResetEvent(false);

            if (cancel.IsCancellationRequested)
                return false;

            double? slopeValue = null;
            double? zeroValue = null;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                var ieee488 = transport as ITransportIEEE488;
                if (ieee488 == null || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.GetCamibrationResult(ieee488, out slopeValue, out zeroValue) || cancel.IsCancellationRequested)
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
            var isCommpete = new AutoResetEvent(false);
            if (cancel.IsCancellationRequested)
                return false;
            _loops.StartMiddleAction(_loopKey, (transport) =>
            {
                var ieee488 = transport as ITransportIEEE488;
                if (ieee488 == null || cancel.IsCancellationRequested)
                {
                    isCommpete.Set();
                    return;
                }

                if (!_adts.SetCalibrationAccept(ieee488, accept) || cancel.IsCancellationRequested)
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

        public double? Pressure{get { return _pressure; }}

        public double? Pitot{get { return _pitot; }}

        public event Action<DateTime> StatusReaded;

        public event Action<DateTime> StateReaded;

        public event Action<DateTime> PressureReaded;

        public event Action<DateTime> PitotReaded;

        #region Service members

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
        #endregion

        /// <summary>
        /// Ожидать статус ADTS
        /// </summary>
        /// <param name="waitFunc">Функция проверки статуса</param>
        /// <returns></returns>
        private EventWaitHandle WaitStatusADTS(Func<Status, bool> waitFunc)
        {
            if (waitFunc == null)
                return null;
            var wh = new AutoResetEvent(false);
            lock (_waitStatusPool)
            {
                _waitStatusPool.Add(wh, waitFunc);
            }
            return wh;
        }

        void AutoUpdateStatus(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.GetStatus(ieee488, out _status))
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

            _loops.StartUnimportantAction(_loopKey, AutoUpdateStatus);
        }

        void AutoUpdateState(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.GetState(ieee488, out _state))
                return;
            _stateTime = DateTime.Now;
            OnStateReaded(_stateTime.Value);
            if (!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(_loopKey, AutoUpdateState);
        }

        void AutoUpdatePressure(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.ReadMeasure(ieee488, Parameters.PS, out _pressure))
                return;
            _pressureTime = DateTime.Now;
            OnPressureReaded(_pressureTime.Value);
            if (!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(_loopKey, AutoUpdatePressure);
        }

        void AutoUpdatePitot(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.ReadMeasure(ieee488, Parameters.PT, out _pitot))
                return;
            _pitotTime = DateTime.Now;
            OnPitotReaded(_pitotTime.Value);
            if (!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(_loopKey, AutoUpdatePitot);
        }

        void UpdateUnit(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if (ieee488 == null)
                return;

            if (!_adts.GetUnits(ieee488, out _pressureUnit))
                return;
            _pressureUnitTime = DateTime.Now;
        }
        #endregion


    }
}
