using System.Collections.Generic;
using ArchiveData.DTO;

namespace PressureSensorCheck.Data
{
    /// <summary>
    /// Конфигурация проверки
    /// </summary>
    public class PressureSensorConfig
    {
        /// <summary>
        /// Точки проверки
        /// </summary>
        public List<PressureSensorPoint> Points { get; set; }

        /// <summary>
        /// Описатель канала давления
        /// </summary>
        public ChannelDescriptor ChannelFrom { get; set; }

        /// <summary>
        /// Описатель преобразованного канала
        /// </summary>
        public ChannelDescriptor ChannelTo { get; set; }
    }
}