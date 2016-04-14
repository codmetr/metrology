using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using IEEE488;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using KipTM.Settings;
using MainLoop;
using NLog;
using PACESeries;

namespace KipTM.Model
{
    public class DeviceManager : IDeviceManager, IDisposable
    {
        private readonly NLog.Logger _logger;

        private readonly ILoops _loops = new Loops();

        private readonly PACE5000Model _paceModel;
        private readonly ADTSModel _adtsModel;

        private readonly IDictionary<string, IEthalonChannel> _ethalonChannels;

        private readonly IDictionary<string, Tuple<ITransportIEEE488, SerialPort>> _ports = new Dictionary<string, Tuple<ITransportIEEE488, SerialPort>>();


        public DeviceManager(ComPortSettings portAdts, ComPortSettings portPace, DeviceSettings pace, DeviceSettings adts, Logger logger = null)
        {
            _logger = logger;

            int address;
            _loops = new Loops();

            // ADTS
            ADTS.ADTSDriver adtsDriver;
            if (int.TryParse(adts.Address, out address))
                adtsDriver = new ADTSDriver(address);
            else
                throw new Exception(string.Format("Can not parse address for ADTS from \"{0}\"", adts.Address ?? "NULL"));
            
            var serialName = string.Format("COM{0}", portAdts.NumberCom);
            var port = new SerialPort(serialName, portAdts.Rate, portAdts.Parity, portAdts.CountBits,
                portAdts.CountStopBits);
            //ITransportIEEE488 ieee488 = new TransportIEEE488(port);
            ITransportIEEE488 ieee488 = new FakeTransport();//todo Заменить на настоящий транспорт
            _ports.Add(serialName, new Tuple<ITransportIEEE488, SerialPort>(ieee488, port));
            _loops.AddLocker(serialName, ieee488);
            _adtsModel = new ADTSModel(adts.Name, _loops, serialName, adtsDriver);

            // PACE
            PACEDriver paceDriver;
            if (int.TryParse(pace.Address, out address))
            {
                var transport = new PACEVisaTransport(address);
                paceDriver = new PACEDriver(address, transport);
            }
            else
                throw new Exception(string.Format("Can not parse address for PACE from \"{0}\"", pace.Address ?? "NULL"));

            if (portAdts.NumberCom != portPace.NumberCom)
            {
                serialName = string.Format("COM{0}", portPace.NumberCom);
                port = new SerialPort(serialName, portPace.Rate, portPace.Parity, portPace.CountBits,
                    portPace.CountStopBits);
                ieee488 = new TransportIEEE488(port);
                _ports.Add(serialName, new Tuple<ITransportIEEE488, SerialPort>(ieee488, port));
                _loops.AddLocker(serialName, ieee488);
            }
            _paceModel = new PACE5000Model("PACE 5000 - модульный контроллер давления/цифровой манометр", _loops, serialName, paceDriver);

            _ethalonChannels = new Dictionary<string, IEthalonChannel>()
            {
                {PACE5000Model.Key, new PACEEchalonChannel(_paceModel)}
            };
        }

        public void Init()
        {
            _adtsModel.Init();
            StartAutoUpdate();
        }

        #region IDeviceManager

        public IEthalonChannel GetEthalonChannel(string deviceKey, object settongs)
        {
            throw new NotImplementedException();
        }

        public PACE5000Model Pace5000
        {
            get { return _paceModel; }
        }

        public ADTSModel ADTS
        {
            get { return _adtsModel; }
        }

        public IDictionary<string, IEthalonChannel> EthalonChannels
        {
            get { return _ethalonChannels; }
        }

        /// <summary>
        /// Запуск автоопроса модуля дискретных входов
        /// </summary>
        public void StartAutoUpdate()
        {
            _adtsModel.StartAutoUpdate();
        }

        /// <summary>
        /// Остановка автоопроса модуля дискретных входов
        /// </summary>
        public void StopAutoUpdate()
        {
            _adtsModel.StopAutoUpdate();
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

    }
}
