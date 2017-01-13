using System;
using System.Collections.Generic;
using System.Linq;
using CheckFrame;
using KipTM.DataService;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;
using MainLoop;
using NLog;

namespace KipTM.Model
{
    public class DeviceManager : IDeviceManager, IDisposable
    {
        private readonly NLog.Logger _logger;

        private readonly ILoops _loops = new Loops();

        private readonly IDictionary<string, IEthalonCannelFactory> _ethalonChannels;

        private IDictionary<Type, IDeviceModelFactory> _modelFabrics;

        private IDictionary<Type, IDeviceFactory> _devicesFabrics;

        private IDictionary<string, IDeviceConfig> _channelsFabrics;

        /// <summary>
        /// Cache devices
        /// </summary>
        private IDictionary<DeviceCacheKey, object> _devicesCache = new Dictionary<DeviceCacheKey, object>();

        /// <summary>
        /// Cache models
        /// </summary>
        private IDictionary<Type, object> _modelsCache = new Dictionary<Type, object>();

        /// <summary>
        /// Device pool, make model, driver and channel
        /// </summary>
        /// <param name="features"></param>
        public DeviceManager(FeatureDescriptorsCombiner features)
        {
            _logger = NLog.LogManager.GetLogger("DeviceManager");

            _loops = new Loops();

            _modelFabrics = features.Models.ToDictionary(el => el.Key, el => el.Value);

            _devicesFabrics = features.Devices.ToDictionary(el => el.Key, el => el.Value);

            _channelsFabrics = features.DeviceConfigs.ToDictionary(el => el.Key, el => el.Value);

            foreach (var fabric in features.DeviceConfigs)
            {
                _loops.AddLocker(fabric.Key, new object());
            }

            _ethalonChannels = features.EthalonChannels.ToDictionary(el => el.Key, el => el.Value);
        }

        #region IDeviceManager

        public IEthalonChannel GetEthalonChannel(string deviceKey)
        {
            var model = GetModel(_ethalonChannels[deviceKey].ModelType);
            return _ethalonChannels[deviceKey].GetChanel(model);
        }

        public T GetModel<T>()
        {
            return (T)GetModel(typeof(T));
        }

        private object GetModel(Type modelType)
        {
            if (!_modelFabrics.ContainsKey(modelType))
                throw new IndexOutOfRangeException(string.Format("For type [{0}] not found fabric", modelType));

            if (!_modelsCache.ContainsKey(modelType))
                _modelsCache[modelType] = _modelFabrics[modelType].GetModel(_loops, this);

            return _modelsCache[modelType];
        }

        public T GetDevice<T>(ITransportChannelType transportDescription)
        {
            if(!_devicesFabrics.ContainsKey(typeof(T)))
                throw new IndexOutOfRangeException(string.Format("For type [{0}] not found fabric", typeof(T)));

            if (!_channelsFabrics.ContainsKey(transportDescription.Key))
                throw new IndexOutOfRangeException(string.Format("For channel [{0}] not found fabric", transportDescription.Key));
            var key = new DeviceCacheKey(typeof (T), transportDescription);
            if (!_devicesCache.ContainsKey(key))
            {
                var chann = _channelsFabrics[transportDescription.Key].GetDriver(transportDescription.Settings);
                _devicesCache.Add(key, _devicesFabrics[typeof(T)].GetDevice(chann));
            }

            return (T)_devicesCache[key];
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
