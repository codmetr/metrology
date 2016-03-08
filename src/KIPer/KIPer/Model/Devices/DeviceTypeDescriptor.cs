using KipTM.ViewModel;

namespace KipTM.Model.Devices
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
        /// Модель прибора
        /// </summary>
        public string Model { get; private set; }

        /// <summary>
        /// Класс устройств
        /// </summary>
        public string DeviceCommonType { get; private set; }

        /// <summary>
        /// Изготовитель
        /// </summary>
        public string DeviceManufacturer { get; private set; }
    }
}
