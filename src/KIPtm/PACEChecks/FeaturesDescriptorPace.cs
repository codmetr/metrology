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
            EtalonTypes = new List<DeviceTypeDescriptor>()
            {
                PACE1000Model.Descriptor
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

            EtalonChannels = new List<KeyValuePair<string, IEtalonCannelFactory>>()
            {
                new KeyValuePair<string, IEtalonCannelFactory>(PACEData.KeysDic.PACE1000Pressure, new PaceEtalonChannelFactory())
            };
        }
        
        /// <summary>
        /// Описатель поддерживаемых типов устройств
        /// </summary>
        public IEnumerable<DeviceTypeDescriptor> DeviceTypes { get; private set; }
        /// <summary>
        /// Описатель поддерживаемых типов эталонов
        /// </summary>
        public IEnumerable<DeviceTypeDescriptor> EtalonTypes { get; private set; }
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
        public IEnumerable<KeyValuePair<string, IEtalonCannelFactory>> EtalonChannels { get; private set; }
        /// <summary>
        /// Получить набор поддерживаемых типов проверок по типам устройств
        /// </summary>
        /// <returns></returns>
        public IDictionary<DeviceTypeDescriptor, IEnumerable<string>> GetDefaultForCheckTypes()
        {
            return new Dictionary<DeviceTypeDescriptor, IEnumerable<string>>() {
            { PACE1000Model.Descriptor, new List<string>(){}}};
        }
    }
}
