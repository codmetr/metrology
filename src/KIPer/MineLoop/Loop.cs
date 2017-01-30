using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace MainLoop
{
    public class Loops : ILoops
    {
        /// <summary>
        /// Коллекция разделяемых ресурсов
        /// </summary>
        /// <remarks>
        /// Такими ресурсами в цикле опроса могут быть порты
        /// </remarks>
        private readonly IDictionary<string, LoopDescriptor> _lockers;

        /// <summary>
        /// Набор потоков работы с разделяемыми ресурсами (1 поток на каждый ресурс)
        /// </summary>
        private readonly IDictionary<string, Thread> _threads;

        /// <summary>
        /// Набор жетонов отмены для потоков работы с ремурсами
        /// </summary>
        private readonly IDictionary<string, CancellationTokenSource> _cancelThreadCollection;

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        public Loops()
        {
            _lockers = new ConcurrentDictionary<string, LoopDescriptor>();
            _cancelThreadCollection = new ConcurrentDictionary<string, CancellationTokenSource>();
            _threads = new ConcurrentDictionary<string, Thread>();
        }

        /// <summary>
        /// Добавить разделяемый ресурс и его ключ
        /// </summary>
        /// <param name="key">ключ ресурса</param>
        /// <param name="locker">разделяемый ресурс</param>
        /// <param name="initAction">метод инициализации локера (если он необходим)</param>
        public void AddLocker(string key, object locker, Action<object> initAction)
        {
            if (_lockers.ContainsKey(key))
                return;

            var cancel = new CancellationTokenSource();
            var parameter = new LoopDescriptor(locker, cancel.Token, initAction, key);
            _lockers.Add(key, parameter);
            _cancelThreadCollection.Add(key, cancel);
            var thread = new Thread(WorkLoop) {Name = string.Format("query loop by [{0}]", key)};
            thread.Start(parameter);

            _threads.Add(key, thread);
        }

        /// <summary>
        /// Добавить разделяемый ресурс и его ключ
        /// </summary>
        /// <param name="key">ключ ресурса</param>
        /// <param name="locker">разделяемый ресурс</param>
        public void AddLocker(string key, object locker)
        {
            AddLocker(key, locker, null);
        }

        /// <summary>
        /// Рабочий цикл для локера
        /// </summary>
        /// <param name="parameter">descripdor</param>
        private void WorkLoop(object parameter)
        {
            var def = parameter as LoopDescriptor;
            if(def==null)
                return;
            while (!def.IsCancel)
            {
                var important = def.GetImportant();
                if (important != null)
                {
                    lock (def.Locker)
                    {
                        if(def.IsNeedInit)
                            def.Init();
                        important(def.Locker);
                    }
                    continue;
                }
                var middle = def.GetMiddle();
                if (middle != null)
                {
                    lock (def.Locker)
                    {
                        if(def.IsNeedInit)
                            def.Init();
                        middle(def.Locker);
                    }
                    continue;
                }
                var unimportant = def.GetUnimportant();
                if (unimportant != null)
                {
                    lock (def.Locker)
                    {
                        if(def.IsNeedInit)
                            def.Init();
                        unimportant(def.Locker);
                    }
                    continue;
                }
                Thread.Sleep(def.Waiting);
                var str = def.Note;
            }
        }

        /// <summary>
        /// Добавить действие в очередь важных действий
        /// </summary>
        /// <param name="key">Ключ локера</param>
        /// <param name="action">действие</param>
        public void StartImportantAction(string key, Action<object> action)
        {
            if(!_threads.ContainsKey(key))
                throw new InvalidProgramException(string.Format("key({0}) not found", key));
            _lockers[key].AddImportant(action);
        }

        /// <summary>
        /// Добавить действие в очередь действий средней важности
        /// </summary>
        /// <param name="key">Ключ локера</param>
        /// <param name="action">действие</param>
        public void StartMiddleAction(string key, Action<object> action)
        {
            if(!_threads.ContainsKey(key))
                throw new InvalidProgramException(string.Format("key({0}) not found", key));
            _lockers[key].AddMiddle(action);
        }

        /// <summary>
        /// Добавить действие в очередь неважных действий
        /// </summary>
        /// <param name="key">Ключ локера</param>
        /// <param name="action">действие</param>
        public void StartUnimportantAction(string key, Action<object> action)
        {
            if(!_threads.ContainsKey(key))
                throw new InvalidProgramException(string.Format("key({0}) not found", key));
            _lockers[key].AddUnimportant(action);
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            foreach (var cancellationTokenSource in _cancelThreadCollection)
            {
                Debug.Write(string.Format("\ncancel loop by key: {0}\n", cancellationTokenSource.Key));
                cancellationTokenSource.Value.Cancel();
            }
        }

        #endregion
    }
}
