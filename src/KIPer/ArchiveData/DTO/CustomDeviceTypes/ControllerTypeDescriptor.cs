using System.Diagnostics;

namespace ArchiveData.DTO.CustomDeviceTypes
{
    /// <summary>
    /// Описатель типа конвертера
    /// </summary>
    [DebuggerDisplay("Converter {DeviceCommonType}: {Model}({DeviceManufacturer})")]
    public class ControllerTypeDescriptor : DeviceTypeDescriptor
    {
        public ControllerTypeDescriptor(string model, string deviceCommonType, string deviceManufacturer)
            :base(model, deviceCommonType, deviceManufacturer)
        {
        }

        /// <summary>
        /// Тип входного параметра
        /// </summary>
        public ChannelType ConvertFrom { get; set; }

        /// <summary>
        /// Тип выходного параметра
        /// </summary>
        public ChannelType ConvertTo { get; set; }

        /// <summary>
        /// Минимальное значение входного диапазона
        /// </summary>
        public double MaxFrom { get; set; }

        /// <summary>
        /// Максимальное значение входного диапазона
        /// </summary>
        public double MinFrom { get; set; }

        /// <summary>
        /// Минимальное значение выходного диапазона
        /// </summary>
        public double MaxTo { get; set; }

        /// <summary>
        /// Максимальное значение выходного диапазона
        /// </summary>
        public double MinTo { get; set; }
    }
}
