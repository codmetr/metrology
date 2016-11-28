namespace KipTM.Settings
{
    /// <summary>
    /// Фабрика настроек проверяемого устройства
    /// </summary>
    public interface IDeviceSettingsFactory
    {
        DeviceSettings GetDefault();
    }
}