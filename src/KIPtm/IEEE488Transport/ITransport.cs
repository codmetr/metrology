namespace IEEE488
{
    public interface ITransport
    {
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