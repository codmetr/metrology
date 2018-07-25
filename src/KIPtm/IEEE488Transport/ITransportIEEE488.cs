namespace IEEE488
{
    public interface ITransportIEEE488
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
        
        /// <summary>
        /// Посылка команды
        /// </summary>
        /// <param name="data">Команда</param>
        /// <returns>Удалось послать команду</returns>
        bool Send(string data);

        /// <summary>
        /// Чтение ответа
        /// </summary>
        /// <returns>Ответ</returns>
        string Receive();
    }
}