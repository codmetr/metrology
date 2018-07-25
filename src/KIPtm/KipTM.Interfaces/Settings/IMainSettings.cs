using System.Collections.Generic;
using ArchiveData.DTO;
using KipTM.Interfaces.Settings;

namespace KipTM.Settings
{
    public interface IMainSettings
    {
        /// <summary>
        /// Типы проверяемых устройств и их измерительные каналы
        /// </summary>
        List<DeviceTypeDescriptor> Devices { get; }
        
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