using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveData.DTO
{
    /// <summary>
    /// Описатель устройства с измерительным каналом
    /// </summary>
    public class DeviceWithChannel
    {
        /// <summary>
        /// Устройство
        /// </summary>
        public DeviceDescriptor Device { get; set; }

        /// <summary>
        /// Измерительный канал
        /// </summary>
        public ChannelDescriptor Channel { get; set; }
    }
}
