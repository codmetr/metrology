using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Model.TransportChannels;
using MainLoop;
using PACESeries;

namespace KipTM.Model.Devices
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

        internal static string Key{get { return "PACE1000"; }}
        internal static string Model { get { return "PACE1000"; } }
        internal static string DeviceCommonType { get { return "Калибратор давления"; } }
        internal static string DeviceManufacturer { get { return "GE Druk"; } }
        internal static IEnumerable<string> TypesEtalonParameters = new[] { "давление", "авиационная высота", "авиационная скорость" };

        #region public properties

        public double Pressure { get; set; }

        public PressureUnits PressureUnit { get; set; }

        public TimeSpan AutoreadPeriod { get { return _periodAutoUpdate; } }

        #endregion

        /// <summary>
        /// Инициализация
        /// </summary>
        public void Start(ITransportChannelType transport)
        {
            _loopKey = transport.Key;
            _driver = _deviceManager.GetDevice<PACE1000Driver>(transport);
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

        public void UpdatePressure()
        {
            _loops.StartMiddleAction(_loopKey, (mb) => _updatePressure(_cancellation.Token));
        }

        public void UpdatePressure(EventWaitHandle wh)
        {
            _loops.StartMiddleAction(_loopKey, (mb) =>
            {
                _updatePressure(_cancellation.Token);
                wh.Set();
            });
        }
        public event EventHandler PressureUnitChanged;

        public event EventHandler PressureChanged;

        protected virtual void OnPressureUnitChanged()
        {
            EventHandler handler = PressureUnitChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnPressureChanged()
        {
            EventHandler handler = PressureChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

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
            
            if (cancel.IsCancellationRequested)
                return;
            var unit = _driver.GetPressureUnit();
            if (unit != null)
                PressureUnit = unit.Value;
        }
    }
}
