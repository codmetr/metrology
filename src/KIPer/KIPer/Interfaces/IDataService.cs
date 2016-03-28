using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KipTM.Archive.DTO;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Settings;

namespace KipTM.Interfaces
{
    public interface IDataService
    {
        /// <summary>
        /// Пул сконфигурируемых устройств
        /// </summary>
        IDeviceManager DeviceManager { get; }
        /// <summary>
        /// Список типов поддерживаемых устройств
        /// </summary>
        IEnumerable<IDeviceTypeDescriptor> DeviceTypes { get; }
        /// <summary>
        /// Список типов поддерживаемых эталонов
        /// </summary>
        IEnumerable<IDeviceTypeDescriptor> EtalonTypes { get; }
        /// <summary>
        /// Архив результатов проверок
        /// </summary>
        ResultsArchive ResultsArchive { get; }
        /// <summary>
        /// Настройки
        /// </summary>
        MainSettings Settings { get; }
        void SaveSettings();
        void LoadResults();
        void SaveResults();
        void InitDevices();
    }
}
