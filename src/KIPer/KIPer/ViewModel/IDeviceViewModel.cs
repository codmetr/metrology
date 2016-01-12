namespace KIPer.ViewModel
{
    public interface IDeviceViewModel
    {
        /// <summary>
        /// Модель прибора
        /// </summary>
        string Model { get; set; }

        /// <summary>
        /// Класс устройств
        /// </summary>
        string DeviceCommonType { get; set; }

        /// <summary>
        /// Изготовитель
        /// </summary>
        string DeviceManufacturer { get; set; }

        /// <summary>
        /// Серийный номер
        /// </summary>
        string SerialNumber { get; set; }
    }
}