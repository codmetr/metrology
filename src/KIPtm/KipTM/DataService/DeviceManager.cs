﻿using System;
using System.Collections.Generic;
using System.Linq;
using CheckFrame;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.TransportChannels;
using MainLoop;

namespace KipTM.DataService
{
    public class DeviceManager : IDeviceManager, IDisposable
    {
        private readonly NLog.Logger _logger;

        private readonly ILoops _loops;

        private readonly IDictionary<string, IEtalonCannelFactory> _etalonChannels;

        private IDictionary<Type, IDeviceModelFactory> _modelFactories;

        private IDictionary<Type, IDeviceFactory> _devicesFactories;

        private IDictionary<string, IDeviceConfig> _configFactories;

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

            _modelFactories = features.Models.ToDictionary(el => el.Key, el => el.Value);

            _devicesFactories = features.Devices.ToDictionary(el => el.Key, el => el.Value);

            _configFactories = features.DeviceConfigs.ToDictionary(el => el.Key, el => el.Value);

            foreach (var feature in features.DeviceConfigs)
            {
                _loops.AddLocker(feature.Key, new object());
            }

            _etalonChannels = features.EtalonChannels.ToDictionary(el => el.Key, el => el.Value);
        }

        #region IDeviceManager
        /// <summary>
        /// Получить канал по типу
        /// </summary>
        /// <param name="deviceKey"></param>
        /// <returns></returns>
        public IEtalonChannel GetEtalonChannel(string deviceKey)
        {
            var model = GetModel(_etalonChannels[deviceKey].ModelType);
            return _etalonChannels[deviceKey].GetChanel(model);
        }

        /// <summary>
        /// Получить визуальную модель для канала
        /// </summary>
        /// <param name="deviceKey">Ключь типа канала</param>
        /// <param name="channel">Канал</param>
        /// <returns></returns>
        public object GetEtalonChannelViewModel(string deviceKey, IEtalonChannel channel)
        {
            return _etalonChannels[deviceKey].GetChanelViewModel(channel);
        }

        public T GetModel<T>()
        {
            return (T)GetModel(typeof(T));
        }

        private object GetModel(Type modelType)
        {
            if (!_modelFactories.ContainsKey(modelType))
                throw new IndexOutOfRangeException(string.Format("For type [{0}] not found factory", modelType));

            if (!_modelsCache.ContainsKey(modelType))
                _modelsCache[modelType] = _modelFactories[modelType].GetModel(_loops, this);

            return _modelsCache[modelType];
        }

        public T GetDevice<T>(ITransportChannelType transportDescription)
        {
            if(!_devicesFactories.ContainsKey(typeof(T)))
                throw new IndexOutOfRangeException(string.Format("For type [{0}] not found factory", typeof(T)));

            if (!_configFactories.ContainsKey(transportDescription.Key))
                throw new IndexOutOfRangeException(string.Format("For channel [{0}] not found factory", transportDescription.Key));
            var key = new DeviceCacheKey(typeof (T), transportDescription);
            if (!_devicesCache.ContainsKey(key))
            {
                var conf = _configFactories[transportDescription.Key].GetDriver(transportDescription.Settings);
                _devicesCache.Add(key, _devicesFactories[typeof(T)].GetDevice(conf));
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
