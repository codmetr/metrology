﻿using System;
using System.Threading;
using ADTSChecks.Model.Devices;
using CheckFrame.Model.Checks.EventArgs;
using CheckFrame.Model.Checks.Steps;
using NLog;
using Tools;

namespace ADTSChecks.Model.Steps.ADTSTest
{
    class EndStep : TestStep, IToBaseStep
    {
        public const string KeyStep = "EndStep";
        private readonly ADTSModel _adts;
        private readonly NLog.Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public EndStep(string name, ADTSModel adts, Logger logger)
        {
            Name = name;
            _adts = adts;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
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
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            _logger.With(l => l.Trace(string.Format("ADTS test end (Go to Ground)")));
            OnProgressChanged(new EventArgProgress(0, "Остановка Поверки"));
            if (!_adts.GoToGround(cancel))
            {
                if(!cancel.IsCancellationRequested)
                    _logger.With(l => l.Trace(string.Format("[ERROR] go to ground")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                whEnd.Set();
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                whEnd.Set();
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Поверка завершена")));
            whEnd.Set();
            OnEnd(new EventArgEnd(KeyStep, true));
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