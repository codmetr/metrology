﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MainLoop
{
    internal class LoopDescriptor
    {
        private CancellationToken _cancel;
        private readonly object locker;
        private readonly Action<object> _initAction;
        private bool _isNeedInit = false;
        private string _note;

        private readonly TimeSpan waiting;

        private readonly Queue<ActionItem<object>> importantActions;
        private readonly Queue<ActionItem<object>> middleActions;
        private readonly Queue<ActionItem<object>> unimportantActions;

        private int _importantCount = 0;
        private int _middleCount = 0;
        private int _unimportantCount = 0;


        #region Constructors
        public LoopDescriptor()
        {
            _initAction = null;
            waiting = TimeSpan.FromMilliseconds(10);
            importantActions = new Queue<ActionItem<object>>();
            middleActions = new Queue<ActionItem<object>>();
            unimportantActions = new Queue<ActionItem<object>>();
        }

        public LoopDescriptor(object locker, CancellationToken cancel, Action<object> initAction = null, string note = null)
            : this()
        {
            _initAction = initAction;
            this.locker = locker;
            this._cancel = cancel;
            _isNeedInit = _initAction != null;
            _note = note;
        }

        public LoopDescriptor(object locker, CancellationToken cancel, Action<object> initAction, TimeSpan waiting)
            : this(locker, cancel, initAction)
        {
            this.waiting = waiting;
        }
        #endregion

        /// <summary>
        /// For locker need call Init action
        /// </summary>
        public bool IsNeedInit { get { return _isNeedInit; } }

        /// <summary>
        /// Node
        /// </summary>
        public string Note{get { return _note; }}

        /// <summary>
        /// Init locker
        /// </summary>
        public void Init()
        {
            if (!_isNeedInit)
                return;
            _isNeedInit = false;
            if (_initAction != null)
                _initAction(this.locker);

        }

        /// <summary>
        /// Add important action with locker
        /// </summary>
        /// <param name="action"></param>
        public void AddImportant(Action<object> action)
        {
            lock (importantActions)
            {
                importantActions.Enqueue(new ActionItem<object>(action));
                _importantCount++;
            }
        }

        /// <summary>
        /// Add important action with locker
        /// </summary>
        /// <param name="action"></param>
        public void AddImportant(Action<object> action, EventWaitHandle wh)
        {
            lock (importantActions)
            {
                importantActions.Enqueue(new ActionItem<object>(action, wh));
                _importantCount++;
            }
        }

        /// <summary>
        /// Add middle action with locker
        /// </summary>
        /// <param name="action"></param>
        public void AddMiddle(Action<object> action)
        {
            lock (middleActions)
            {
                middleActions.Enqueue(new ActionItem<object>(action));
                _middleCount++;
            }
        }

        /// <summary>
        /// Add middle action with locker
        /// </summary>
        /// <param name="action"></param>
        public void AddMiddle(Action<object> action, EventWaitHandle wh)
        {
            lock (middleActions)
            {
                middleActions.Enqueue(new ActionItem<object>(action, wh));
                _middleCount++;
            }
        }

        /// <summary>
        /// Add unimportant action with locker
        /// </summary>
        /// <param name="action"></param>
        public void AddUnimportant(Action<object> action)
        {
            lock (unimportantActions)
            {
                unimportantActions.Enqueue(new ActionItem<object>(action));
                _unimportantCount++;
            }
        }

        /// <summary>
        /// Add unimportant action with locker
        /// </summary>
        /// <param name="action"></param>
        public void AddUnimportant(Action<object> action, EventWaitHandle wh)
        {
            lock (unimportantActions)
            {
                unimportantActions.Enqueue(new ActionItem<object>(action, wh));
                _unimportantCount++;
            }
        }

        /// <summary>
        /// Get next important action from pool
        /// </summary>
        /// <returns>important action</returns>
        public ActionItem<object> GetImportant()
        {
            if (_importantCount <= 0)
                return null;
            ActionItem<object> result;
            lock (importantActions)
            {
                result = importantActions.Dequeue();
                _importantCount--;
            }
            return result;
        }

        /// <summary>
        /// Get next middle action from pool
        /// </summary>
        /// <returns>middle action</returns>
        public ActionItem<object> GetMiddle()
        {
            if (_middleCount <= 0)
                return null;
            ActionItem<object> result;
            lock (middleActions)
            {
                result = middleActions.Dequeue();
                _middleCount--;
            }
            return result;
        }

        /// <summary>
        /// Get next unimportant action from pool
        /// </summary>
        /// <returns>unimportant action</returns>
        public ActionItem<object> GetUnimportant()
        {
            if (_unimportantCount <= 0)
                return null;
            ActionItem<object> result;
            lock (unimportantActions)
            {
                result = unimportantActions.Dequeue();
                _unimportantCount--;
            }
            return result;
        }

        /// <summary>
        /// Need cancel all action for this locker 
        /// </summary>
        public bool IsCancel { get { return _cancel.IsCancellationRequested; } }

        /// <summary>
        /// Locker
        /// </summary>
        public Object Locker { get { return locker; } }

        /// <summary>
        /// Wainting interval in work loop
        /// </summary>
        public TimeSpan Waiting { get { return waiting; } }

    }
}