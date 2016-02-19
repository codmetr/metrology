using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
        }

        #region IDeviceManager

        /// <summary>
        /// Запуск автоопроса модуля дискретных входов
        /// </summary>
        public void StartAutoUpdate()
        {
            _isNeedAutoupdate = true;
        }

        /// <summary>
        /// Остановка автоопроса модуля дискретных входов
        /// </summary>
        public void StopAutoUpdate()
        {
            _isNeedAutoupdate = false;
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
            _loops.Dispose();
            if (!disposeAll)
                return;
        }
        #endregion

        #region Service members

        void AutoUpdate(object transport)
        {
        }

        #endregion
    }
}
