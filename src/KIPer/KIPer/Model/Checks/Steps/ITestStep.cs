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
        string Name { get;}

        void Start(EventWaitHandle whEnd);

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
        /// Получены какие-то результаты шага
        /// </summary>
        event EventHandler<EventArgError> Error;

        /// <summary>
        /// Шаг окончен
        /// </summary>
        event EventHandler<EventArgEnd> End;
    }
}
