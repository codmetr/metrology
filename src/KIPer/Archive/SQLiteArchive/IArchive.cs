namespace SQLiteArchive
{
    /// <summary>
    /// Архив
    /// </summary>
    public interface IArchive
    {
        /// <summary>
        /// Записать в архив
        /// </summary>
        /// <typeparam name="T">Тип записывапемых данных</typeparam>
        /// <param name="key">Ключ данных</param>
        /// <param name="entity">Записываемые даннные</param>
        void Save<T>(string key, T entity);

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