using System;
using System.Threading;

namespace CheckFrame.Model.Checks.Steps
{
    /// <summary>
    /// Шаг подерживает паузу
    /// </summary>
    public interface IPausedStep
    {
        /// <summary>
        /// Указывает что вызов паузы допустим
        /// </summary>
        /// <returns></returns>
        bool IsPauseAvailable { get; }

        /// <summary>
        /// Поставить на паузу
        /// </summary>
        /// <returns></returns>
        bool Pause(CancellationToken cancel);

        /// <summary>
        /// Восстановить с паузы
        /// </summary>
        /// <returns></returns>
        bool Resume(CancellationToken cancel);

        /// <summary>
        /// Изменилась доступность вызова паузы
        /// </summary>
        event EventHandler PauseAccessibilityChanged;
    }
}