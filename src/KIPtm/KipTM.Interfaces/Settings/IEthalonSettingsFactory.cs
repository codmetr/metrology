namespace KipTM.Settings
{
    /// <summary>
    /// Фабрика настроек эталона по умолчанию
    /// </summary>
    public interface IEtalonSettingsFactory
    {
        /// <summary>
        /// Получить настройки эталона по умолчанию
        /// </summary>
        /// <returns></returns>
        DeviceSettings GetDefault(); 
    }
}