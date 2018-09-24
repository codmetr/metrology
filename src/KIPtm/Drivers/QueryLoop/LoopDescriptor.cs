using System;
using System.Collections.Generic;
using System.Threading;

namespace QueryLoop
{
    internal class LoopDescriptor
    {
        private CancellationToken _cancel;
        private readonly object locker;
        private readonly Action<object> _initAction;
        private bool _isNeedInit;

        private readonly TimeSpan _waiting;

        private readonly Queue<Action<object>> importantActions;
        private readonly Queue<Action<object>> middleActions;
        private readonly Queue<Action<object>> unimportantActions;

        private int _importantCount;
        private int _middleCount;
        private int _unimportantCount;

        public TimeSpan DefaulLoopPeriod { get { return TimeSpan.FromMilliseconds(10); } }

        #region Constructors

        public LoopDescriptor()
        {
            _initAction = null;
            _waiting = DefaulLoopPeriod;
            importantActions = new Queue<Action<object>>();
            middleActions = new Queue<Action<object>>();
            unimportantActions = new Queue<Action<object>>();
        }

        public LoopDescriptor(object locker, CancellationToken cancel, Action<object> initAction = null)
            : this() 
        {
            _initAction = initAction;
            this.locker = locker;
            _cancel = cancel;
            _isNeedInit = _initAction != null;
        }

        public LoopDescriptor(object locker, CancellationToken cancel, Action<object> initAction, TimeSpan? waiting)
            : this(locker, cancel, initAction) 
        {
            _waiting = waiting ?? DefaulLoopPeriod;
        }

        #endregion

        /// <summary>
        /// For locker need call Init action
        /// </summary>
        public bool IsNeedInit { get { return _isNeedInit; } }

        /// <summary>
        /// Init locker
        /// </summary>
        public void Init()
        {
            if (!_isNeedInit)
                return;
            _isNeedInit = false;
            if (_initAction != null)
                _initAction(locker);

        }

        /// <summary>
        /// Add important action with locker
        /// </summary>
        /// <param name="action"></param>
        public void AddImportant(Action<object> action)
        {
            lock (importantActions)
            {
                importantActions.Enqueue(action);
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
                middleActions.Enqueue(action);
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
                unimportantActions.Enqueue(action);
                _unimportantCount++;
            }
        }

        /// <summary>
        /// Get next important action from pool
        /// </summary>
        /// <returns>important action</returns>
        public Action<object> GetImportant()
        {
            if (_importantCount <= 0)
                return null;
            Action<object> result;
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
        public Action<object> GetMiddle()
        {
            if (_middleCount <= 0)
                return null;
            Action<object> result;
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
        public Action<object> GetUnimportant()
        {
            if (_unimportantCount <= 0)
                return null;
            Action<object> result;
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
        public TimeSpan Waiting { get { return _waiting; } }
    }
}