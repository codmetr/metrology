using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using IEEE488;
using MainLoop;
using NLog;
using PACESeries;

namespace KipTM.Model
{
    public class DeviceManager
    {
        private readonly NLog.Logger _logger;

        private readonly ILoops _loops = new Loops();

        private DateTime? _pressureTime;
        private double? _pressure;

        private DateTime? _pitotTime;
        private double? _pitot;

        private DateTime? _pressureUnitTime;
        private PressureUnits? _pressureUnit;

        private DateTime? _stateTime;
        private State? _state;

        private readonly ADTS.ADTSDriver _adts;
        private readonly PACEDriver _pace;

        private bool _isNeedAutoupdate;
        private readonly IDictionary<string, Tuple<ITransportIEEE488, SerialPort>> _ports = new Dictionary<string, Tuple<ITransportIEEE488, SerialPort>>();


        public DeviceManager(ADTSDriver adts, PACEDriver pace, Logger logger = null)
        {
            _adts = adts;
            _pace = pace;
            _logger = logger;
        }

        public void Init()
        {
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, UpdateUnit);
            StartAutoUpdate();
        }

        #region IDeviceManager

        /// <summary>
        /// Запуск автоопроса модуля дискретных входов
        /// </summary>
        public void StartAutoUpdate()
        {
            _isNeedAutoupdate = true;
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdateState);
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdatePressure);
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdatePitot);
        }

        /// <summary>
        /// Остановка автоопроса модуля дискретных входов
        /// </summary>
        public void StopAutoUpdate()
        {
            _isNeedAutoupdate = false;
        }

        public DateTime? StartCalibration(CalibChannel channel)
        {
            var isCommpete = new AutoResetEvent(false);
            DateTime? date = null;
            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, (transport) =>
            {
                var ieee488 = transport as ITransportIEEE488;
                if (ieee488 == null)
                {
                    isCommpete.Set();
                    return;
                }

                _adts.GetDate(ieee488, out date);

                isCommpete.Set();
            });
            isCommpete.WaitOne();
            return date;
        }
        #endregion

        #region implementation IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposeAll">
        /// false - should only clean up native resources
        /// true - should clean up both managed and native resources
        /// </param>
        protected virtual void Dispose(bool disposeAll)
        {
            StopAutoUpdate();
            _loops.Dispose();
            if (!disposeAll)
                return;
        }
        #endregion

        #region Service members

        void AutoUpdateState(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if(ieee488==null)
                return;

            if(!_isNeedAutoupdate)
                return;
            if(!_adts.GetState(ieee488, out _state))
                return;
            _stateTime = DateTime.Now;
            if(!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdateState);
        }

        void AutoUpdatePressure(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if(ieee488==null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.ReadMeasure(ieee488, Parameters.PS,  out _pressure))
                return;
            _pressureTime = DateTime.Now;
            if (!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdatePressure);
        }

        void AutoUpdatePitot(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if(ieee488==null)
                return;

            if (!_isNeedAutoupdate)
                return;
            if (!_adts.ReadMeasure(ieee488, Parameters.PT, out _pitot))
                return;
            _pitotTime = DateTime.Now;
            if (!_isNeedAutoupdate)
                return;

            _loops.StartUnimportantAction(KipTM.Enums.KeyPortADTS, AutoUpdatePitot);
        }

        void UpdateUnit(object transport)
        {
            var ieee488 = transport as ITransportIEEE488;
            if(ieee488==null)
                return;

            if (!_adts.GetUnits(ieee488, out _pressureUnit))
                return;
            _pressureUnitTime = DateTime.Now;
        }
        #endregion
    }
}
