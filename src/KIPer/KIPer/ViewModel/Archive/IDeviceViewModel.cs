namespace KipTM.ViewModel
{
    /// <summary>
    /// Идентификатор конкретного устройства
    /// </summary>
    public interface IDeviceViewModel
    {
        /// <summary>
        /// Тип устройства
        /// </summary>
        IDeviceTypeDescriptor DeviceType { get; set; }

        /// <summary>
        /// Серийный номер
        /// </summary>
        string SerialNumber { get; set; }
    }
}