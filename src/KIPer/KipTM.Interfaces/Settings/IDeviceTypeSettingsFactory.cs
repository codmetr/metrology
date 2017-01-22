using System.Collections.Generic;

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
        IEnumerable<DeviceTypeSettings> GetDefault();
    }
}