using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using IEEE488;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Settings;
using MainLoop;
using NLog;
using PACESeries;
using PACEVISADriver;

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

        private readonly IDictionary<ITransportChannelType, object> _devicesOnPorts = new Dictionary<ITransportChannelType, object>();

        private IDictionary<Type, Func<object, object>> _devicesFabrics;

        private IDictionary<string, Func<object, ITransportIEEE488>> _channelsFabrics;


        public DeviceManager(Logger logger = null)
        {
            _logger = logger;

            _loops = new Loops();

            _devicesFabrics = new Dictionary<Type, Func<object, object>>()
            {
                {
                    typeof(ADTSDriver),
                    options =>
                        {
                            var tupleParam = options as Tuple<int, ITransportIEEE488>;
                            if (tupleParam == null)
                                throw new TargetParameterCountException(string.Format(
                                    "option mast be type: {0}; now type: {1}",
                                    typeof (Tuple<int, ITransportIEEE488>), options.GetType()));
                            return new ADTSDriver(tupleParam.Item1, tupleParam.Item2);
                        }
                },

                {
                    typeof(PASE1000Driver),
                    options =>
                        {
                            var tupleParam = options as Tuple<int, ITransportIEEE488>;
                            if (tupleParam == null)
                                throw new TargetParameterCountException(string.Format(
                                    "option mast be type: {0}; now type: {1}",
                                    typeof (Tuple<int, ITransportIEEE488>), options.GetType()));
                            return new PASE1000Driver(tupleParam.Item1, tupleParam.Item2);
                        }
                },
            };

            _channelsFabrics = new Dictionary<string, Func<object, ITransportIEEE488>>()
            {
                {VisaChannelDescriptor.KeyType, opt =>
                {
                    return new FakeTransport();//todo Заменить на настоящий транспорт
                    return new VisaIEEE488();
                }}
            };


            _adtsModel = new ADTSModel(string.Format("{0} {1}", ADTSModel.Model, ADTSModel.DeviceCommonType), _loops,
                VisaChannelDescriptor.KeyType, this);

            _paceModel = new PACE5000Model(
                string.Format("{0} {1}", PACE5000Model.Model, PACE5000Model.DeviceCommonType), _loops,
                VisaChannelDescriptor.KeyType, this);

            _ethalonChannels = new Dictionary<string, IEthalonChannel>()
            {
                {PACE5000Model.Key, new PACEEchalonChannel(_paceModel)}
            };
        }

        #region IDeviceManager

        public IEthalonChannel GetEthalonChannel(string deviceKey, object settings)
        {
            return _ethalonChannels[deviceKey];
        }

        public T GetDevice<T>(int address, ITransportChannelType transportDescription)
        {
            if(!_devicesFabrics.ContainsKey(typeof(T)))
                throw new IndexOutOfRangeException(string.Format("For type [{0}] not found fabric", typeof(T)));

            if (!_channelsFabrics.ContainsKey(transportDescription.Key))
                throw new IndexOutOfRangeException(string.Format("For channel [{0}] not found fabric", transportDescription.Key));

            var chann = _channelsFabrics[transportDescription.Key](transportDescription.Settings);

            return (T)_devicesFabrics[typeof (T)](new Tuple<int, ITransportIEEE488>(address, chann));
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

    }
}
