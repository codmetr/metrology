namespace KIPer.ViewModel
{
    /// <summary>
    /// Идентификатор конкретного устройства
    /// </summary>
    public interface IDeviceViewModel
    {
        /// <summary>
        /// Тип устройства
        /// </summary>
        DeviceTypeDescriptor DeviceType { get; set; }

        /// <summary>
        /// Серийный номер
        /// </summary>
        string SerialNumber { get; set; }
    }
}