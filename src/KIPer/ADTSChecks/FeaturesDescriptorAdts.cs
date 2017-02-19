﻿using System;
using System.Collections.Generic;
using ADTS;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ArchiveData.DTO;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.ViewModel.Checks;

namespace ADTSChecks
{
    public class FeaturesDescriptorAdts : IFeaturesDescriptor
    {
        public FeaturesDescriptorAdts()
        {
            DeviceTypes = new List<DeviceTypeDescriptor>()
            {
                new DeviceTypeDescriptor(ADTSModel.Model, ADTSModel.DeviceCommonType, ADTSModel.DeviceManufacturer)
            };
            EthalonTypes = new List<DeviceTypeDescriptor>()
            {
            };
            Models = new Dictionary<Type, IDeviceModelFactory>()
            {
                {typeof(ADTSModel), new ADTSModelFactory()},
            };
            Devices = new List<KeyValuePair<Type, IDeviceFactory>>()
            {
                new KeyValuePair<Type, IDeviceFactory>(typeof(ADTSDriver), new ADTSFactory()),
            };
            var factoriesVisa = new VisaChannel.ChannelsFactory();
            ChannelFactories = factoriesVisa;

            var driversConf = new List<KeyValuePair<string, IDeviceConfig>>();
            driversConf.AddRange(factoriesVisa.GetDevicesConfig());
            DeviceConfigs = driversConf;

            EthalonChannels = new List<KeyValuePair<string, IEthalonCannelFactory>>()
            {
            };
        }
        
        /// <summary>
        /// Описатель поддерживаемых типов устройств
        /// </summary>
        public IEnumerable<DeviceTypeDescriptor> DeviceTypes { get; private set; }
        /// <summary>
        /// Описатель поддерживаемых типов эталонов
        /// </summary>
        public IEnumerable<DeviceTypeDescriptor> EthalonTypes { get; private set; }
        /// <summary>
        /// Фабрика каналов
        /// </summary>
        public IChannelsFactory ChannelFactories { get; private set; }
        /// <summary>
        /// Фабрики моделей для типов устройств
        /// </summary>
        public IEnumerable<KeyValuePair<Type, IDeviceModelFactory>> Models { get; private set; }
        /// <summary>
        /// Фабрики драйверов устройств
        /// </summary>
        public IEnumerable<KeyValuePair<Type, IDeviceFactory>> Devices { get; private set; }
        /// <summary>
        /// Фабрики каналов проверяемых устройств
        /// </summary>
        public IEnumerable<KeyValuePair<string, IDeviceConfig>> DeviceConfigs { get; private set; }
        /// <summary>
        /// Фабрики каналов эталонов
        /// </summary>
        public IEnumerable<KeyValuePair<string, IEthalonCannelFactory>> EthalonChannels { get; private set; }
        /// <summary>
        /// Получить набор поддерживаемых типов проверок по типам устройств
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ArchivedKeyValuePair> GetDefaultForCheckTypes()
        {
            return new List<ArchivedKeyValuePair>
            {
                new ArchivedKeyValuePair(ADTSModel.Key, new List<string>()
                {
                    Calibration.key,
                }),
            };
        }
    }
}