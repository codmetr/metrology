﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;

namespace KipTM.Settings
{
    public class DeviceSettings
    {
        public string Name;

        /// <summary>
        /// Описатель устройства
        /// </summary>
        public DeviceTypeDescriptor TypeDescriptor;

        ///// <summary>
        ///// Модель прибора
        ///// </summary>
        //public string Model;
        //
        ///// <summary>
        ///// Класс устройств
        ///// </summary>
        //public string DeviceCommonType;
        //
        ///// <summary>
        ///// Изготовитель
        ///// </summary>
        //public string DeviceManufacturer;

        /// <summary>
        /// Типы параметров
        /// </summary>
        public List<string> TypesEtalonParameters;

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber;

        public string Address;
        
        public string NamePort;
    }
}
