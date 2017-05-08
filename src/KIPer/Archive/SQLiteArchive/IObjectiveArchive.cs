using System;
using System.Collections.Generic;

namespace SQLiteArchive
{
    /// <summary>
    /// Хранилище данных
    /// </summary>
    public interface IObjectiveArchive
    {
        /// <summary>
        /// Создает новую запись о проверке и возвращает её ID
        /// </summary>
        /// <returns></returns>
        int CreateNewRepair(DateTime timestamp);

        /// <summary>
        /// Получить ID всех проверок
        /// </summary>
        /// <returns>список ID всех проверок</returns>
        IEnumerable<Repair> LoadAllRepairs();

        /// <summary>
        /// Сохранить результат по проверке
        /// </summary>
        /// <typeparam name="T">тип результата</typeparam>
        /// <param name="repairId">ID проверки</param>
        /// <param name="result">результат</param>
        void SaveResult<T>(int repairId, T result);

        /// <summary>
        /// Загрузить результат проверки
        /// </summary>
        /// <typeparam name="T">тип результата</typeparam>
        /// <param name="repairId">ID проверки</param>
        T LoadResult<T>(int repairId) where T : class;

        /// <summary>
        /// Сохранить результат по проверке
        /// </summary>
        /// <typeparam name="T">тип результата</typeparam>
        /// <param name="repairId">ID проверки</param>
        /// <param name="parameters">результат</param>
        void SaveParameters<T>(int repairId, T parameters);

        /// <summary>
        /// Загрузить результат проверки
        /// </summary>
        /// <typeparam name="T">тип результата</typeparam>
        /// <param name="repairId">ID проверки</param>
        T LoadParameters<T>(int repairId) where T : class;

        /// <summary>
        /// Добавить или обновить метаданные по проверке
        /// </summary>
        /// <param name="repairId">ID проверки</param>
        /// <param name="key">ключь метаданных</param>
        /// <param name="val">метаданные</param>
        void AddOrUpdateMetadata(int repairId, string key, string val);

        /// <summary>
        /// Получить все метаданные по проверке
        /// </summary>
        /// <param name="repairId">ID проверки</param>
        IDictionary<string, string> GetAllMetadata(int repairId);

    }
}