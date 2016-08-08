using System.Collections.Generic;

namespace KipTM.Archive
{
    public interface IPropertyPool
    {
        /// <summary>
        /// Получить свойство проверки
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetProperty<T>(string key);

        /// <summary>
        /// Получить список всех ключей
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAllKeys();

        /// <summary>
        /// Получить хранилище свойств по ключу
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IPropertyPool ByKey(string key);
    }
}
