using System;
using System.Collections.Generic;
using ArchiveData.DTO;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using PACEChecks.Channels;
using PACEChecks.Devices;
using PACESeries;

namespace PACEChecks
{
    public class FeaturesDescriptorPace : IFeaturesDescriptor
    {
        public FeaturesDescriptorPace()
        {
            DeviceTypes = new List<DeviceTypeDescriptor>()
            {
                //todo добавить устройство, когда будет методика проверки
            };
            EthalonTypes = new List<DeviceTypeDescriptor>()
            {
                new DeviceTypeDescriptor(PACE1000Model.Model, PACE1000Model.DeviceCommonType, PACE1000Model.DeviceManufacturer)
            };
            Models = new Dictionary<Type, IDeviceModelFactory>()
            {
                {typeof(PACE1000Model), new PACE1000ModelFactory()},
            };
            Devices = new List<KeyValuePair<Type, IDeviceFactory>>()
            {
                new KeyValuePair<Type, IDeviceFactory>(typeof(PACE1000Driver), new PACE1000Factory()),
            };
            var factoriesVisa = new VisaChannel.ChannelsFactory();
            ChannelFactories = factoriesVisa;

            var driversConf = new List<KeyValuePair<string, IDeviceConfig>>();
            driversConf.AddRange(factoriesVisa.GetDevicesConfig());
            DeviceConfigs = driversConf;

            EthalonChannels = new List<KeyValuePair<string, IEthalonCannelFactory>>()
            {
                new KeyValuePair<string, IEthalonCannelFactory>(PACE1000Model.Key, new PACEEthalonChannelFactory())
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
                new ArchivedKeyValuePair(PACE1000Model.Key, new List<string>()),
            };
        }
    }
}
