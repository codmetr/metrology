﻿using System;
using System.Collections.Generic;
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
            _lockers = new Dictionary<string, LoopDescriptor>();
            _cancelThreadCollection = new Dictionary<string, CancellationTokenSource>();
            _threads = new Dictionary<string, Thread>();
        }

        /// <summary>
        /// Добавить разделяемый ресурс и его ключ
        /// </summary>
        /// <param name="key">ключ ресурса</param>
        /// <param name="locker">разделяемый ресурс</param>
        public void AddLocker(string key, object locker)
        {
            var cancel = new CancellationTokenSource();
            var parameter = new LoopDescriptor(locker, cancel.Token);
            _lockers.Add(key, parameter);
            _cancelThreadCollection.Add(key, cancel);
            var thread = new Thread(WorkLoop);
            thread.Start(parameter);
            _threads.Add(key, thread);
        }

        /// <summary>
        /// Main loop
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
                        important(def.Locker);
                    }
                    continue;
                }
                var middle = def.GetMiddle();
                if (middle != null)
                {
                    lock (def.Locker)
                    {
                        middle(def.Locker);
                    }
                    continue;
                }
                var unimportant = def.GetUnimportant();
                if (unimportant != null)
                {
                    lock (def.Locker)
                    {
                        unimportant(def.Locker);
                    }
                    continue;
                }
                Thread.Sleep(def.Waiting);
            }
        }

        /// <summary>
        /// Добавить 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void StartImportantAction(string key, Action<object> action)
        {
            if(!_threads.ContainsKey(key))
                throw new InvalidProgramException(string.Format("key({0}) not found", key));
            _lockers[key].AddImportant(action);
        }

        public void StartMiddleAction(string key, Action<object> action)
        {
            if(!_threads.ContainsKey(key))
                throw new InvalidProgramException(string.Format("key({0}) not found", key));
            _lockers[key].AddMiddle(action);
        }

        public void StartUnmportantAction(string key, Action<object> action)
        {
            if(!_threads.ContainsKey(key))
                throw new InvalidProgramException(string.Format("key({0}) not found", key));
            _lockers[key].AddUnimportant(action);
        }
    }
}
