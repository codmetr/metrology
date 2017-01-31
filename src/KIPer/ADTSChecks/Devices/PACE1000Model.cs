using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ADTSChecks.Devices;
using ADTSData;
using CheckFrame.Model;
using KipTM.Model;
using KipTM.Model.TransportChannels;
using MainLoop;
using PACESeries;

namespace ADTSChecks.Model.Devices
{
    public class PACE1000Model
    {
        private IDeviceManager _deviceManager;
        private PACESeries.PACE1000Driver _driver;
        private ILoops _loops;
        private string _loopKey;
        private bool _autoUpdateRun = false;
        private TimeSpan _periodAutoUpdate = TimeSpan.FromMilliseconds(200);
        private CancellationTokenSource _cancellationAutoread = new CancellationTokenSource();
        private CancellationTokenSource _cancellation = new CancellationTokenSource();
        private double _pressure;
        private PressureUnits _pressureUnit;

        public PACE1000Model(string title, ILoops loops, IDeviceManager deviceManager)
        {
            Title = title;
            _loops = loops;
            _deviceManager = deviceManager;
        }

        public string Title
        {
            get;
            private set;
        }

        public static string Key { get { return "PACE1000"; } }
        public static string Model { get { return KeysDic.PACE1000ModelKey; } }
        public static string DeviceCommonType { get { return "Калибратор давления"; } }
        public static string DeviceManufacturer { get { return "GE Druk"; } }
        public static IEnumerable<string> TypesEtalonParameters = new[] { "давление", "авиационная высота", "авиационная скорость" };

        /// <summary>
        /// Инициализация
        /// </summary>
        public void Start(ITransportChannelType transport)
        {
            _loopKey = transport.Key;
            _driver = _deviceManager.GetDevice<PACE1000Driver>(transport);
        }

        #region Autoread

        public TimeSpan AutoreadPeriod { get { return _periodAutoUpdate; } }

        public void StartAutoread(TimeSpan autoreadPeriod)
        {
            SetAutoreadPeriod(autoreadPeriod);
            if (_autoUpdateRun)
                return;

            CancellationToken cancel = _cancellationAutoread.Token;
            Task.Factory.StartNew((cncl)=>AutoreadFunction((CancellationToken)cncl), (object)cancel, cancel);
        }

        public void StopAutoUpdate()
        {
            _cancellationAutoread.Cancel();
            _cancellationAutoread = new CancellationTokenSource();
        }

        public void SetAutoreadPeriod(TimeSpan autoreadPeriod)
        {
            _periodAutoUpdate = autoreadPeriod;
        }

        #endregion

        #region Pressure Unit

        public PressureUnits PressureUnit
        {
            get { return _pressureUnit; }
            set
            {
                if(value==_pressureUnit)
                    return;
                _pressureUnit = value; 
                OnPressureUnitChanged();
            }
        }

        public void SetPressureUnit(PressureUnits unit)
        {
            _loops.StartMiddleAction(_loopKey, (mb) =>
            {
                if(!_driver.SetPressureUnit(unit))
                    return;
                PressureUnit = unit;
            });
        }

        public void UpdateUnit()
        {
            _loops.StartMiddleAction(_loopKey, (mb) => _updateUnit(_cancellation.Token));
        }

        /// <summary>
        /// Обновить значение единиц измерения
        /// </summary>
        /// <param name="wh">Событие синхронизации</param>
        /// <returns>Контейнер результата</returns>
        public QueryResult UpdateUnit(EventWaitHandle wh)
        {
            return AsyncWrapper(() => _updateUnit(_cancellation.Token), wh);
        }

        public event EventHandler PressureUnitChanged;

        protected virtual void OnPressureUnitChanged()
        {
            EventHandler handler = PressureUnitChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion

        #region Pressure

        public double Pressure
        {
            get { return _pressure; }
            set
            {
                _pressure = value; 
                OnPressureChanged();
            }
        }

        /// <summary>
        /// Обновить значение давления
        /// </summary>
        public bool UpdatePressure()
        {
            return AsyncWrapper(() => _loops.StartMiddleAction(_loopKey, (mb) => _updatePressure(_cancellation.Token))).IsSuccess;
        }

        /// <summary>
        /// Обновить значение давления
        /// </summary>
        /// <param name="wh">Событие синхронизации</param>
        /// <returns>Контейнер результата</returns>
        public QueryResult UpdatePressure(EventWaitHandle wh)
        {
            return AsyncWrapper(() => _loops.StartMiddleAction(_loopKey, (mb) => _updatePressure(_cancellation.Token)), wh);
        }

        public event EventHandler PressureChanged;

        protected virtual void OnPressureChanged()
        {
            EventHandler handler = PressureChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion

        #region Local/Remote

        public void SetLloOn()
        {
            _loops.StartMiddleAction(_loopKey, (mb) => _driver.SetLocalLockOutMode());
        }

        public void SetLloOff()
        {
            _loops.StartMiddleAction(_loopKey, (mb) => _driver.SetOffLocalLockOutMode());
        }

        public void SetLocal()
        {
            _loops.StartMiddleAction(_loopKey, (mb) => _driver.SetLocal());
        }

        public void SetRemote()
        {
            _loops.StartMiddleAction(_loopKey, (mb) => _driver.SetRemote());
        }

        #endregion

        #region _Services
        private void AutoreadFunction(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return;
            _loops.StartMiddleAction(_loopKey, (mb) => _updatePressure(cancel));

            var timeSetToQueue = DateTime.Now;
            while (!cancel.IsCancellationRequested && (DateTime.Now - timeSetToQueue)<_periodAutoUpdate)
            {
                Thread.Sleep(10);
            }

            if (!cancel.IsCancellationRequested)
            {
                _loops.StartMiddleAction(_loopKey, (mb) => AutoreadFunction(cancel));
            }
        }

        private void _updatePressure(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return;
            var pressure = _driver.GetPressure();
            if (!double.IsNaN(pressure))
                Pressure = pressure;
        }

        private void _updateUnit(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return;
            var unit = _driver.GetPressureUnit();
            if (unit != null)
                PressureUnit = unit.Value;
        }

        /// <summary>
        /// Синхронизирующая обертка
        /// </summary>
        /// <param name="act"></param>
        /// <param name="wh"></param>
        /// <returns></returns>
        private QueryResult AsyncWrapper(Action act, EventWaitHandle wh)
        {
            var result = new QueryResult();
            _loops.StartMiddleAction(_loopKey, (mb) =>
            {
                try
                {
                    act();
                    result.IsSuccess = true;
                }
                catch (Exception e)
                {
                    result.IsSuccess = false;
                    result.ErrorNote = e.Message;
                    result.Arg = e;
                }
                wh.Set();
            });
            return result;
        }

        /// <summary>
        /// Синхронизирующая обертка
        /// </summary>
        /// <param name="act"></param>
        /// <param name="wh"></param>
        /// <returns></returns>
        private QueryResult AsyncWrapper(Action act)
        {
            var wh = new ManualResetEvent(false);
            var res = AsyncWrapper(act, wh);
            wh.WaitOne();
            return res;
        }
        #endregion
    }
}
