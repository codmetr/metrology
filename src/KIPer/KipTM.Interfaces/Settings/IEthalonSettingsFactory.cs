namespace KipTM.Settings
{
    /// <summary>
    /// Фабрика настроек эталона
    /// </summary>
    public interface IEthalonSettingsFactory
    {
        DeviceSettings GetDefault(); 
    }
}