using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using KipTM.Archive;

namespace KipTM.Model.Checks
{
    public interface ICheckMethod
    {
        /// <summary>
        /// Название методики
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        bool Init(IPropertyPool propertyes);

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