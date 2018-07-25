using System;
using System.Threading;
using ADTSChecks.Devices;
using CheckFrame.Checks.Steps;
using CheckFrame.Model.Checks.Steps;
using KipTM.Model.Checks;
using KipTM.Model.Checks.Steps;
using NLog;
using Tools;

namespace ADTSChecks.Model.Steps.ADTSCalibration
{
    class ToBaseStep : TestStep, IToBaseStep
    {
        public const string KeyStep = "ToBaseStep";

        private readonly ADTSModel _adts;
        private readonly NLog.Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;
        
        public ToBaseStep(string name, ADTSModel adts, Logger logger)
        {
            Name = name;
            _adts = adts;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override void Start(CancellationToken cancel)
        {
            DateTime testDate = DateTime.Now;
            OnStarted();
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            _logger.With(l => l.Trace(string.Format("ADTS test end (Go to Ground)")));
            OnProgressChanged(new EventArgProgress(0, "Перевод в базовое состояние"));
            if (!_adts.GoToGround(cancel))
            {
                if (!cancel.IsCancellationRequested)
                    _logger.With(l => l.Trace(string.Format("[ERROR] go to ground")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("В базовом состоянии")));
            OnEnd(new EventArgEnd(KeyStep, true));
            return;
        }
    }
}
