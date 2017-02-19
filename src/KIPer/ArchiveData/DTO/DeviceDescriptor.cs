using System;
using System.Diagnostics;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Базовый описатель устройства
    /// </summary>
    [DebuggerDisplay("{DeviceType.Model}: {SerialNumber})")]
    public class DeviceDescriptor
    {
        public DeviceDescriptor(DeviceTypeDescriptor deviceType)
        {
            DeviceType = deviceType;
            PreviousCheckTime = DateTime.Now;
        }

        /// <summary>
        /// для сериализатора
        /// </summary>
        public DeviceDescriptor():this(null)
        {
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
