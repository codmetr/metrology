using System;
using System.Collections.Generic;
using ArchiveData.DTO;
using KipTM.Archive.DataTypes;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces
{
    /// <summary>
    /// Описатель возможности по проверке
    /// </summary>
    public interface IFeaturesDescriptor
    {
        /// <summary>
        /// Описатель поддерживаемых типов устройств
        /// </summary>
        IEnumerable<DeviceTypeDescriptor> DeviceTypes { get; }
        /// <summary>
        /// Описатель поддерживаемых типов эталонов
        /// </summary>
        IEnumerable<DeviceTypeDescriptor> EthalonTypes { get; }
        /// <summary>
        /// Фабрика каналов
        /// </summary>
        IChannelsFactory ChannelFactories { get; }
        /// <summary>
        /// Фабрики моделей для типов устройств
        /// </summary>
        IEnumerable<KeyValuePair<Type, IDeviceModelFactory>> Models { get; }
        /// <summary>
        /// Фабрики драйверов устройств
        /// </summary>
        IEnumerable<KeyValuePair<Type, IDeviceFactory>> Devices { get; }
        /// <summary>
        /// Фабрики настройки каналов проверяемых устройств
        /// </summary>
        IEnumerable<KeyValuePair<string, IDeviceConfig>> DeviceConfigs { get; }
        /// <summary>
        /// Фабрики каналов эталонов
        /// </summary>
        IEnumerable<KeyValuePair<string, IEthalonCannelFactory>> EthalonChannels { get; }
        /// <summary>
        /// Получить набор поддерживаемых типов проверок по типам устройств
        /// </summary>
        /// <returns></returns>
        IEnumerable<ArchivedKeyValuePair> GetDefaultForCheckTypes();
    }
}