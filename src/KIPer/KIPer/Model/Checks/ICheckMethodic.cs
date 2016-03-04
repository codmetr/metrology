using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace KipTM.Model.Checks
{
    public interface ICheckMethodic
    {
        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        bool Init(IDictionary<string, object> parameters);

        /// <summary>
        /// Запуск калибровки
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Список шагов
        /// </summary>
        IEnumerable<ITestStep> Steps { get; } 

        /// <summary>
        /// Остановка проверки
        /// </summary>
        void Stop();
    }
}