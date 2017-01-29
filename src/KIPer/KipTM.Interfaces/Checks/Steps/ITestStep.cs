using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public interface ITestStep
    {
        /// <summary>
        /// Название шага
        /// </summary>
        string Name { get;}

        /// <summary>
        /// Запустить шаг
        /// </summary>
        /// <param name="whEnd"></param>
        void Start(EventWaitHandle whEnd);

        /// <summary>
        /// Остановить шаг
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// Шаг запущен
        /// </summary>
        event EventHandler<EventArgs> Started;

        /// <summary>
        /// Шаг запущен
        /// </summary>
        event EventHandler<EventArgStepResult> ResultUpdated;

        /// <summary>
        /// Изменение прогресса шага (0-100 %)
        /// </summary>
        event EventHandler<EventArgProgress> ProgressChanged;

        /// <summary>
        /// Получена ошибка
        /// </summary>
        event EventHandler<EventArgError> Error;

        /// <summary>
        /// Шаг окончен
        /// </summary>
        event EventHandler<EventArgEnd> End;
    }
}
