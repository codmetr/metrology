namespace IEEE488
{
    public interface ITransportIEEE488:ITransport
    {
        /// <summary>
        /// Открыть подключение
        /// </summary>
        /// <param name="address">Адрес прибора</param>
        /// <returns>true - Удалось подключиться</returns>
        bool Open(int address);
        
        /// <summary>
        /// Закрыть подключение
        /// </summary>
        /// <param name="address"></param>
        /// <returns>true - Удалось отключиться без ошибок</returns>
        bool Close(int address);
    }
}