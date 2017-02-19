using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model.TransportChannels;

namespace CheckFrame
{
    public class FeatureDescriptorsCombiner// : IFeaturesDescriptor
    {
        private readonly IEnumerable<IFeaturesDescriptor> _features;

        public FeatureDescriptorsCombiner(IEnumerable<IFeaturesDescriptor> features)
        {
            _features = features;
            DeviceTypes = _features.SelectMany(el => el.DeviceTypes);
            EthalonTypes = _features.SelectMany(el => el.EthalonTypes);
            Models = _features.SelectMany(el => el.Models);
            Devices = _features.SelectMany(el => el.Devices);
            DeviceConfigs = UnionByKey(_features, f => f.DeviceConfigs, (ch1, ch2) => ch1.Key == ch2.Key);
            //ChannelFactories = _features.Select(el => el.ChannelFactories);
            ChannelFactories = new ChannelFactoryCombiner(_features);
            EthalonChannels = UnionByKey(_features, f=>f.EthalonChannels, (ch1, ch2)=>ch1.Key==ch2.Key);
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
        public IChannelsFactory ChannelFactories { get; private set;  }
        /// <summary>
        /// Фабрики моделей для типов устройств
        /// </summary>
        public IEnumerable<KeyValuePair<Type, IDeviceModelFactory>> Models { get; private set; }
        /// <summary>
        /// Фабрики драйверов устройств
        /// </summary>
        public IEnumerable<KeyValuePair<Type, IDeviceFactory>> Devices { get; private set; }
        /// <summary>
        /// Фабрики настройки каналов проверяемых устройств
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
            return _features.SelectMany(el => el.GetDefaultForCheckTypes());
        }

        public IEnumerable<IFeaturesDescriptor> Features { get { return _features; } }

        internal static IEnumerable<T> UnionByKey<T>(IEnumerable<IFeaturesDescriptor> futures,
            Func<IFeaturesDescriptor, IEnumerable<T>> getAim, Func<T, T, bool> equal)
        {
            var result = new List<T>();
            foreach (var future in futures)
            {
                var items = getAim(future);
                foreach (var item in items)
                {
                    if (result.Any(el => equal(el, item)))
                        continue;
                    result.Add(item);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Надстройка для получения списка неповторяющихся каналов
    /// </summary>
    internal class ChannelFactoryCombiner: IChannelsFactory
    {
        private readonly IEnumerable<IFeaturesDescriptor> _features;

        public ChannelFactoryCombiner(IEnumerable<IFeaturesDescriptor> features)
        {
            _features = features;
        }

        public IEnumerable<ITransportChannelType> GetChannels()
        {
            return FeatureDescriptorsCombiner.UnionByKey(_features,
                (features) => features.ChannelFactories.GetChannels(),
                (ch1, ch2) => ch1.Key == ch2.Key);
        }
    }
}
