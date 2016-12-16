using System;
using System.Collections.Generic;
using ADTS;
using ADTSChecks.Channels;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ArchiveData.DTO;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.ViewModel.Checks;
using PACESeries;

namespace ADTSChecks
{
    class FeaturesDescriptor : IFeaturesDescriptor
    {
        public FeaturesDescriptor()
        {
            DeviceTypes = new List<DeviceTypeDescriptor>()
            {
                new DeviceTypeDescriptor(ADTSModel.Model, ADTSModel.DeviceCommonType, ADTSModel.DeviceManufacturer)
            };
            EthalonTypes = new List<DeviceTypeDescriptor>()
            {
                new DeviceTypeDescriptor(PACE1000Model.Model, PACE1000Model.DeviceCommonType, PACE1000Model.DeviceManufacturer)
            };
            Models = new Dictionary<Type, IDeviceModelFactory>()
            {
                {typeof(ADTSModel), new ADTSModelFactory()},
                {typeof(PACE1000Model), new PACE1000ModelFactory()},
            };
            Devices = new List<KeyValuePair<Type, IDeviceFactory>>()
            {
                new KeyValuePair<Type, IDeviceFactory>(typeof(ADTSDriver), new ADTSFactory()),
                new KeyValuePair<Type, IDeviceFactory>(typeof(PACE1000Driver), new PACE1000Factory()),
            };
            var channelsFactories = new List<KeyValuePair<string, IChannelFactory>>();
            var factoriesVisa = new VisaChannel.ChannelsFactory();
            channelsFactories.AddRange(factoriesVisa.GetChannels());
            ChannelsFactories = channelsFactories;

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
        public IEnumerable<KeyValuePair<string, IChannelFactory>> ChannelsFactories { get; private set; }
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
                    Calibration.Key,
                }),
            };
        }
    }
}
