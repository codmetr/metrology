using System.Collections.Generic;
using CheckFrame.Archive;

namespace CheckFrame.Model.Checks
{
    public interface ICheckMethod
    {
        /// <summary>
        /// Идентификатор методики
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Название методики
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Получить 
        /// </summary>
        /// <param name="propertyPool"></param>
        /// <returns></returns>
        object GetCustomConfig(IPropertyPool propertyPool);

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        bool Init(object customConf);

        /// <summary>
        /// Запуск калибровки
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Список шагов
        /// </summary>
        IEnumerable<CheckStepConfig> Steps { get; } 

        /// <summary>
        /// Остановка проверки
        /// </summary>
        void Stop();
    }
}