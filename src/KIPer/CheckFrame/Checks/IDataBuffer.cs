using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Буффер данных для обмена между шагами
    /// </summary>
    public interface IDataBuffer
    {
        /// <summary>
        /// Добавить данные в архив
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="data">Данные</param>
        void Append<T>(T data) where T : class;

        /// <summary>
        /// Добавить данные в архив по ключу
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="data">Данные</param>
        /// <param name="key">Ключ</param>
        void Append<T>(T data, string key);

        /// <summary>
        /// Получить данные из архива
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <returns>Результат</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// получить данные из архива по ключу
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="key">Ключ</param>
        /// <returns>Результат</returns>
        T Resolve<T>(string key);

        /// <summary>
        /// Получить данные из архива
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <returns>Результат</returns>
        bool TryResolve<T>(out T res) where T : class;

        /// <summary>
        /// получить данные из архива по ключу
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип данных</typeparam>
        /// <param name="key">Ключ</param>
        /// <returns>Результат</returns>
        bool TryResolve<T>(string key, out T res) where T : class;

        /// <summary>
        /// Отчистка всего справочника
        /// </summary>
        void Clear();
    }
}
