using System.Collections.Generic;

namespace KipTM.Interfaces
{
    public interface ITamplateArchive<T>
    {
        /// <summary>
        /// Прочитать все наборы конфигурации
        /// </summary>
        /// <returns></returns>
        Dictionary<string, T> GetArchive();

        /// <summary>
        /// Загрузить последнюю конфигурацию
        /// </summary>
        /// <returns></returns>
        T GetLast();
        /// <summary>
        /// Сохранить последнюю конфигурацию
        /// </summary>
        /// <param name="data"></param>
        void SetLast(T data);
        /// <summary>
        /// Добавить шаблон
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conf"></param>
        void AddTemplate(string name, T conf);
        /// <summary>
        /// Обновить шаблон
        /// </summary>
        /// <param name="name"></param>
        /// <param name="conf"></param>
        void Update(string name, T conf);
        /// <summary>
        /// Удалить шаблон
        /// </summary>
        /// <param name="name"></param>
        void Remove(string name);
    }
}