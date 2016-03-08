﻿using KipTM.ViewModel;

namespace KipTM.Model.Devices
{
    /// <summary>
    /// Базовый описатель типа устройства
    /// </summary>
    public class DeviceDescriptor
    {
        public DeviceDescriptor(DeviceTypeDescriptor deviceType, string serialNumber)
        {
            SerialNumber = serialNumber;
            DeviceType = deviceType;
        }

        public DeviceTypeDescriptor DeviceType { get; private set; }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber { get; private set; }
    }
}