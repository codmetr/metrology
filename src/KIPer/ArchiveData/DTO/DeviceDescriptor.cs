﻿using System;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Базовый описатель устройства
    /// </summary>
    public class DeviceDescriptor
    {
        public DeviceDescriptor(DeviceTypeDescriptor deviceType)
        {
            DeviceType = deviceType;
        }

        public DeviceTypeDescriptor DeviceType { get; set; }

        /// <summary>
        /// Заводской номер
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime PreviousCheckTime { get; set; }
    }
}
