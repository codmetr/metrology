using System.Collections.Generic;
using ArchiveData.DTO;
using KipTM.Interfaces.Settings;

namespace KipTM.Settings
{
    /// <summary>
    /// Типы проверяемых устройств и их измерительные каналы
    /// </summary>
    public interface IDeviceTypeSettingsFactory
    {
        /// <summary>
        /// Типы проверяемых устройств и их измерительные каналы
        /// </summary>
        /// <returns></returns>
        IEnumerable<DeviceTypeDescriptor> GetDefault();
    }
}