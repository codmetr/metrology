using System;

namespace QueryLoop
{
    public interface ILoops : IDisposable 
    {
        /// <summary>
        /// Добавить разделяемый ресурс и его ключ
        /// </summary>
        /// <param name="key">ключ ресурса</param>
        /// <param name="locker">разделяемый ресурс</param>
        void AddLocker(string key, object locker);

        /// <summary>
        /// Добавить разделяемый ресурс и его ключ
        /// </summary>
        /// <param name="key">ключ ресурса</param>
        /// <param name="locker">разделяемый ресурс</param>
        /// <param name="loopPeriod">задержка между циклами опроса очереди</param>
        void AddLocker(string key, object locker, TimeSpan loopPeriod);

        /// <summary>
        /// Добавить разделяемый ресурс и его ключ
        /// </summary>
        /// <param name="key">ключ ресурса</param>
        /// <param name="locker">разделяемый ресурс</param>
        /// <param name="initAction">метод инициализации локера (если он необходим)</param>
        /// <param name="loopPeriod">задержка между циклами опроса очереди</param>
        void AddLocker(string key, object locker, Action<object> initAction, TimeSpan? loopPeriod);

        /// <summary>
        /// Добавить действие в очередь важных действий
        /// </summary>
        /// <param name="key">Ключ локера</param>
        /// <param name="action">действие</param>
        void StartImportantAction(string key, Action<object> action);

        /// <summary>
        /// Добавить действие в очередь действий средней важности
        /// </summary>
        /// <param name="key">Ключ локера</param>
        /// <param name="action">действие</param>
        void StartMiddleAction(string key, Action<object> action);

        /// <summary>
        /// Добавить действие в очередь неважных действий
        /// </summary>
        /// <param name="key">Ключ локера</param>
        /// <param name="action">действие</param>
        void StartUnimportantAction(string key, Action<object> action);

        /// <summary>
        /// Отмена по одному из локеров
        /// </summary>
        /// <param name="key"></param>
        void Cancel(string key);
    }
}