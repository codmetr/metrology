using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tools
{
    public class Watcher
    {
        private readonly Action _checker;
        private readonly Action<Exception> _errorWatcher;
        private CancellationTokenSource _cancellator;
        private Thread _watcherTread = null;
        private static readonly TimeSpan DefaultWatchPeriod = TimeSpan.FromMilliseconds(100);
        private TimeSpan _watchPeriod;
        private readonly TimeSpan _waitPeriod = TimeSpan.FromMilliseconds(10);

        public Watcher(Action checker, Action<Exception> errorWatcher )
        {
            _checker = checker;
            _errorWatcher = errorWatcher;
            _cancellator = new CancellationTokenSource();
            _watchPeriod = DefaultWatchPeriod;
        }

        public void SetWatchPeriod(TimeSpan watchPeriod)
        {
            _watchPeriod = watchPeriod;
        }

        public void ResetWatchPeriod(TimeSpan watchPeriod)
        {
            _watchPeriod = watchPeriod;
        }

        public void StartWatch()
        {
            if(_watcherTread !=null)
                return;
            var cancel = _cancellator.Token;
            _watcherTread = new Thread((param) => Watch((CancellationToken)param));
            _watcherTread.Start(cancel);
        }

        public void StopWatch()
        {
            _cancellator.Cancel();
            _cancellator = new CancellationTokenSource();
        }

        private void Watch(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                try
                {
                    _checker();
                }
                catch (Exception ex)
                {
                    _errorWatcher(ex);
                }
                var start = DateTime.Now;
                var wait = DateTime.Now - start;
                while (wait < _watchPeriod && !cancellation.IsCancellationRequested)
                {
                    Thread.Sleep(_waitPeriod);
                    wait = DateTime.Now - start;
                }
            }
        }
    }
}
