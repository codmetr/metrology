using System;
using System.Collections.Generic;
using ArchiveData.DTO;
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
        /// Фабрики моделей для типов устройств
        /// </summary>
        IEnumerable<KeyValuePair<Type, IDeviceModelFactory>> Models { get; }
        /// <summary>
        /// Фабрики драйверов устройств
        /// </summary>
        IEnumerable<KeyValuePair<Type, IDeviceFactory>> Devices { get; }
        /// <summary>
        /// Фабрики каналов проверяемых устройств
        /// </summary>
        IEnumerable<KeyValuePair<string, Func<object, object>>> ChannelsFabrics { get; }
        /// <summary>
        /// Фабрики каналов эталонов
        /// </summary>
        IEnumerable<KeyValuePair<string, Func<ITransportChannelType, IEthalonChannel>>> EthalonChannels { get; }
    }
}