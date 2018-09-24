using System;

namespace PACESeriesUtil
{
    /// <summary>
    /// Контекст выполнения
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Синхронизация не требуется
        /// </summary>
        bool IsSynchronized { get; }

        /// <summary>
        /// Синхронное выполнение действия в контексте
        /// </summary>
        /// <param name="action">действие</param>
        void Invoke(Action action);

        /// <summary>
        /// Синхронное выполнение действия в контексте
        /// </summary>
        /// <param name="action">действие</param>
        void BeginInvoke(Action action);
    }
}
