using System;
using System.Collections.Generic;
using System.Threading;
using ArchiveData.DTO.Params;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using NLog;
using Tools;

namespace KipTM.Model.Checks.Steps.ADTSCalibration
{
    class ToBase : TestStep, IToBaseStep
    {
        private readonly ADTSModel _adts;
        private readonly NLog.Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _checkCancelPeriod;

        public ToBase(string name, ADTSModel adts, Logger logger)
        {
            Name = name;
            _adts = adts;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
            _checkCancelPeriod = TimeSpan.FromMilliseconds(10);
        }

        public override void Start(EventWaitHandle whEnd)
        {
            var cancel = _cancellationTokenSource.Token;
            DateTime testDate = DateTime.Now;
            OnStarted();
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                whEnd.Set();
                OnEnd(new EventArgEnd(false));
                return;
            }
            _logger.With(l => l.Trace(string.Format("ADTS test end (Go to Ground)")));
            OnProgressChanged(new EventArgProgress(0, "Перевод в базовое состояние"));
            if (!_adts.GoToGround(cancel))
            {
                if (!cancel.IsCancellationRequested)
                    _logger.With(l => l.Trace(string.Format("[ERROR] go to ground")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                whEnd.Set();
                OnEnd(new EventArgEnd(false));
                return;
            }
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                whEnd.Set();
                OnEnd(new EventArgEnd(false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("В базовом состоянии")));
            whEnd.Set();
            OnEnd(new EventArgEnd(true));
            return;
        }

        public override bool Stop()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            return true;
        }
    }
}
