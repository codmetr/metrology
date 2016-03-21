using System;
using System.Collections.Generic;
using System.Threading;

namespace KipTM.Model.Checks.Steps
{
    public abstract class TestStep:ITestStep
    {
        protected string _name;

        /// <summary>
        /// Название шага
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Набор необходимых измерительных и управляющих каналов
        /// </summary>
        //public abstract IEnumerable<string> RequiredChannels { get; }

        /// <summary>
        /// Запустить тест
        /// </summary>
        public abstract void Start(EventWaitHandle whEnd);

        /// <summary>
        /// Остановить тест
        /// </summary>
        public abstract bool Stop();
        
        /// <summary>
        /// Шаг запущен
        /// </summary>
        public event EventHandler<EventArgs> Started;

        /// <summary>
        /// Изменение прогресса шага (0-100 %)
        /// </summary>
        public event EventHandler<EventArgProgress> ProgressChanged;

        /// <summary>
        /// Получены какие-то результаты шага
        /// </summary>
        public event EventHandler<EventArgTestResult> ResultUpdated;

        /// <summary>
        /// Шаг окончен
        /// </summary>
        public event EventHandler<EventArgEnd> End;

        /// <summary>
        /// Возникла ошибка
        /// </summary>
        public event EventHandler<EventArgError> Error;

        #region Инвокаторы

        protected virtual void OnStarted()
        {
            EventHandler<EventArgs> handler = Started;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnProgressChanged(EventArgProgress e)
        {
            EventHandler<EventArgProgress> handler = ProgressChanged;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnResultUpdated(EventArgTestResult e)
        {
            EventHandler<EventArgTestResult> handler = ResultUpdated;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnEnd(EventArgEnd e)
        {
            EventHandler<EventArgEnd> handler = End;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnError(EventArgError e)
        {
            EventHandler<EventArgError> handler = Error;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}
