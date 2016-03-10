using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KipTM.Model;
using KipTM.Model.Archive;
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
        /// Набор сконфигурированных эталонов
        /// </summary>
        IEnumerable<DeviceDescriptor> Etalons { get; }
        /// <summary>
        /// Набор поддерживаемых методик
        /// </summary>
        IDictionary<string, ICheckMethodic> Methodics { get; }
        /// <summary>
        /// Архив результатов проверок
        /// </summary>
        ResultsArchive ResultsArchive { get; }
        /// <summary>
        /// Настройки
        /// </summary>
        MainSettings Settings { get; }
        void LoadSettings();
        void SaveSettings();
        void LoadResults();
        void SaveResults();
        void InitDevices();
    }
}
