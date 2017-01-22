using System.Collections.Generic;

namespace KipTM.Settings
{
    public interface IMainSettings
    {
        /// <summary>
        /// Типы проверяемых устройств и их измерительные каналы
        /// </summary>
        List<DeviceTypeSettings> Devices { get; }
        
        /// <summary>
        /// Последние сконфигурированные эталоны
        /// </summary>
        List<DeviceSettings> LastEtalons { get; }

        /// <summary>
        /// Последние сконфигурированные проверяемые устройства
        /// </summary>
        List<DeviceSettings> LastDevices { get; }

        /// <summary>
        /// Присутствующие в системы порты и их настройки
        /// </summary>
        List<ComPortSettings> Ports { get; }
    }
}