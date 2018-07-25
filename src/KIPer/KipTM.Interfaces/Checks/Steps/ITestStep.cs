using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Interfaces.Checks;

namespace KipTM.Model.Checks
{
    public interface ITestStep<T>: ITestStep
    {
        /// <summary>
        /// Шаг запущен
        /// </summary>
        event EventHandler<EventArgStepResult<T>> ResultUpdated;
    }

    public interface ITestStepWithBuffer: ITestStep
    {
        /// <summary>
        /// Передает буфер для размещения результатов проверки
        /// </summary>
        void SetBuffer(IDataBuffer buffer);
    }

    public interface ITestStep
    {
        /// <summary>
        /// Название шага
        /// </summary>
        string Name { get;}

        /// <summary>
        /// Запустить шаг
        /// </summary>
        /// <param name="cancel"></param>
        void Start(CancellationToken cancel);

        /// <summary>
        /// Шаг запущен
        /// </summary>
        event EventHandler<EventArgs> Started;

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

    /// <summary>
    /// Завершающий шаг
    /// </summary>
    public interface IFinalizeStep { }
}
