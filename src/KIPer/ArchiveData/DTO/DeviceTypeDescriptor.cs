namespace ArchiveData.DTO
{
    /// <summary>
    /// Базовый описатель типа устройства
    /// </summary>
    public class DeviceTypeDescriptor : IDeviceTypeDescriptor
    {
        public DeviceTypeDescriptor(string model, string deviceCommonType, string deviceManufacturer)
        {
            DeviceManufacturer = deviceManufacturer;
            DeviceCommonType = deviceCommonType;
            Model = model;
        }

        /// <summary>
        /// для сериализатора
        /// </summary>
        public DeviceTypeDescriptor()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Модель прибора
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Класс устройств
        /// </summary>
        public string DeviceCommonType { get; set; }

        /// <summary>
        /// Изготовитель
        /// </summary>
        public string DeviceManufacturer { get; set; }

    }
}
