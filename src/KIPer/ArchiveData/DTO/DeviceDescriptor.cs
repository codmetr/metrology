using System;

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
        /// Инвентарный номер
        /// </summary>
        public string InventarNumber { get; set; }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime PreviousCheckTime { get; set; }
    }
}
