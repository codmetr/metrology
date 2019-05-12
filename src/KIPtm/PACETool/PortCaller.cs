using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PACETool
{
    class PortCaller
    {
        private readonly object _locker = new object();
        private readonly CancellationToken _cancel;
        private bool _isAutoUpdate;
        private Action _updateAction;
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private readonly AutoResetEvent _newInQueue = new AutoResetEvent(false);

        public PortCaller(CancellationToken cancel)
        {
            _cancel = cancel;
        }

        public void AddToAutoupdate(Action act, TimeSpan period)
        {
            if(_isAutoUpdate)
                throw new Exception("dublicate call AddToAutoupdate");
            _updateAction = act;
            _isAutoUpdate = true;
            Task tsk = new Task((arg)=> Circle((TimeSpan)arg), period);
            tsk.Start();
        }

        public void StopAutoupdate()
        {
            if(!_isAutoUpdate)
                if (_isAutoUpdate)
                    throw new Exception("dublicate call StopAutoupdate");
            _isAutoUpdate = false;
        }

        public void CallSync(Action act)
        {
            if (_isAutoUpdate)
            {
                _queue.Enqueue(act);
                _newInQueue.Set();
            }
            else
            {
                lock (_locker)
                {
                    act();
                }
            }
        }

        private void Circle(TimeSpan period)
        {
            while (_isAutoUpdate && !_cancel.IsCancellationRequested)
            {
                var res = WaitHandle.WaitAny(new WaitHandle[] {_newInQueue, _cancel.WaitHandle}, period);
                if(res == 1)
                    break;
                if (res == 0 || !_queue.IsEmpty)
                    CallQueue(_queue, _cancel);

                if(_cancel.IsCancellationRequested)
                    break;

                lock (_locker)
                {
                    _updateAction();
                }
            }
            if(_cancel.IsCancellationRequested)
                return;
            if(!_queue.IsEmpty)
                CallQueue(_queue, _cancel);
        }

        private void CallQueue(ConcurrentQueue<Action> queue, CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                Action act;
                if(!queue.TryDequeue(out act))
                    return;
                lock (_locker)
                {
                    act();
                }
            }
        }
    }
}
