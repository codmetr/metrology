﻿namespace ArchiveData.DTO
{
    public interface IDeviceTypeDescriptor
    {
        /// <summary>
        /// Модель прибора
        /// </summary>
        string Model { get; }

        /// <summary>
        /// Класс устройств
        /// </summary>
        string DeviceCommonType { get; }

        /// <summary>
        /// Изготовитель
        /// </summary>
        string Manufacturer { get; }
    }
}