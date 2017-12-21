namespace SQLiteArchive
{
    /// <summary>
    /// Архив
    /// </summary>
    public interface IArchive
    {

        /// <summary>
        /// Прочитать из архива
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="key">Ключ данных</param>
        /// <param name="def">Значение в случае неудачи чтения</param>
        /// <returns>Прочитанные данные</returns>
        T Load<T>(string key, T def);
    }
}