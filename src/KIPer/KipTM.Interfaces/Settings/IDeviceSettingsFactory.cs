namespace KipTM.Settings
{
    /// <summary>
    /// Фабрика настроек проверяемого устройства по умолчанию
    /// </summary>
    public interface IDeviceSettingsFactory
    {
        /// <summary>
        /// Получить настройки проверяемого устройства по умолчанию
        /// </summary>
        /// <returns></returns>
        DeviceSettings GetDefault();
    }
}