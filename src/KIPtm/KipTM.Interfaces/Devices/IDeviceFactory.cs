namespace KipTM.Interfaces.Checks
{
    /// <summary>
    /// Фабрика драйвера устройства
    /// </summary>
    public interface IDeviceFactory
    {
        /// <summary>
        /// Получить драйвер устройства
        /// </summary>
        /// <param name="options">параметры инициализации устройства</param>
        /// <returns>драйвер устройства</returns>
        object GetDevice(object options);
    }
}
