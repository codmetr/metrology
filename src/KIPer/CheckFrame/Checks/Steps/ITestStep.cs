using System;
using System.Threading;
using CheckFrame.Model.Checks.EventArgs;

namespace CheckFrame.Model.Checks.Steps
{
    public interface ITestStep
    {
        string Name { get;}

        void Start(EventWaitHandle whEnd);

        bool Stop();

        /// <summary>
        /// Шаг запущен
        /// </summary>
        event EventHandler<System.EventArgs> Started;

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
