﻿using System;
using System.Threading;
using KipTM.Model.Checks;

namespace KipTM.Interfaces.Checks.Steps
{
    public abstract class TestStep<T>:ITestStep<T>
    {
        protected string _name;

        /// <summary>
        /// Название шага
        /// </summary>
        public string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        /// <summary>
        /// Набор необходимых измерительных и управляющих каналов
        /// </summary>
        //public abstract IEnumerable<string> RequiredChannels { get; }

        /// <summary>
        /// Запустить тест
        /// </summary>
        public abstract void Start(CancellationToken cancel);

        /// <summary>
        /// Шаг запущен
        /// </summary>
        public event EventHandler<System.EventArgs> Started;

        /// <summary>
        /// Изменение прогресса шага (0-100 %)
        /// </summary>
        public event EventHandler<EventArgProgress> ProgressChanged;

        /// <summary>
        /// Получены какие-то результаты шага
        /// </summary>
        public event EventHandler<EventArgStepResult<T>> ResultUpdated;

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
            EventHandler<System.EventArgs> handler = Started;
            if (handler != null) handler(this, System.EventArgs.Empty);
        }

        protected virtual void OnProgressChanged(EventArgProgress e)
        {
            EventHandler<EventArgProgress> handler = ProgressChanged;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnResultUpdated(EventArgStepResult<T> e)
        {
            EventHandler<EventArgStepResult<T>> handler = ResultUpdated;
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
