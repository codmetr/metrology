using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MainLoop
{
    class LoopDescriptor
    {
        private CancellationToken cancel;
        private object locker;
        private Queue<Action<object>> importantActions;
        private Queue<Action<object>> middleActions;
        private Queue<Action<object>> unimportantActions;

        private int _importantCount = 0;
        private int _middleCount = 0;
        private int _unimportantCount = 0;

        public LoopDescriptor(object locker, CancellationToken cancel)
        {
            this.locker = locker;
            this.cancel = cancel;
            importantActions = new Queue<Action<object>>();
            middleActions = new Queue<Action<object>>();
            unimportantActions = new Queue<Action<object>>();

        }

        public void AddImportant(Action<object> action)
        {
            lock (importantActions)
            {
                importantActions.Enqueue(action);
                _importantCount++;
            }
        }

        public void AddMiddle(Action<object> action)
        {
            lock (middleActions)
            {
                middleActions.Enqueue(action);
                _middleCount++;
            }
        }

        public void AddUnimportant(Action<object> action)
        {
            lock (unimportantActions)
            {
                unimportantActions.Enqueue(action);
                _unimportantCount++;
            }
        }

        internal Action<object> GetImportant()
        {
            if(_importantCount<=0)
                return null;
            Action<object> result;
            lock (importantActions)
            {
                result = importantActions.Dequeue();
                _importantCount--;
            }
            return result;
        }

    }
}
