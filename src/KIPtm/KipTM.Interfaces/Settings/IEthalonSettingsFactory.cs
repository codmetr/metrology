namespace KipTM.Settings
{
    /// <summary>
    /// Фабрика настроек эталона по умолчанию
    /// </summary>
    public interface IEthalonSettingsFactory
    {
        /// <summary>
        /// Получить настройки эталона по умолчанию
        /// </summary>
        /// <returns></returns>
        DeviceSettings GetDefault(); 
    }
}