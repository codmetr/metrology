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
        /// Архив результатов проверок
        /// </summary>
        ResultsArchive ResultsArchive { get; }
        /// <summary>
        /// Настройки
        /// </summary>
        IMainSettings Settings { get; }

        /// <summary>
        /// Загрузить результатаы и настройки
        /// </summary>
        void LoadResults();

        /// <summary>
        /// Задать список типов устройств и эталонов
        /// </summary>
        /// <param name="deviceTypes"></param>
        /// <param name="ethalonTypes"></param>
        void FillDeviceList(IEnumerable<DeviceTypeDescriptor> deviceTypes, IEnumerable<DeviceTypeDescriptor> ethalonTypes);
    }
}
