namespace KipTM.Interfaces.Channels
{
    /// <summary>
    /// Фабрика драйверов каналов
    /// </summary>
    public interface IDeviceConfig
    {
        /// <summary>
        /// Получить канал устройства
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        object GetDriver(object opt);
    }
}