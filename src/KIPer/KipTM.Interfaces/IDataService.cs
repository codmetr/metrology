using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Settings;

namespace KipTM.Interfaces
{
    public interface IDataService
    {
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
        IMainSettings Settings { get; }

        /// <summary>
        /// Сохранить текущие настройки
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Загрузить настройки
        /// </summary>
        void LoadResults();

        /// <summary>
        /// Сохранить текущие результаты
        /// </summary>
        void SaveResults();

        /// <summary>
        /// Задать список типов устройств и эталонов
        /// </summary>
        /// <param name="deviceTypes"></param>
        /// <param name="ethalonTypes"></param>
        void FillDeviceList(IEnumerable<DeviceTypeDescriptor> deviceTypes, IEnumerable<DeviceTypeDescriptor> ethalonTypes);
    }
}
