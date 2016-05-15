using System;
using System.Threading;
using ADTS;
using KipTM.Model.Devices;
using KipTM.Model.Params;
using NLog;
using Tools;

namespace KipTM.Model.Checks.Steps.ADTSTest
{
    class Init : TestStep
    {
        private readonly ADTSModel _adts;
        private readonly CalibChannel _calibChan;
        private readonly NLog.Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public Init(string name, ADTSModel adts, CalibChannel calibChan, Logger logger)
        {
            Name = name;
            _adts = adts;
            _calibChan = calibChan;
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
                OnEnd(new EventArgEnd(false));
                return;
            }
            _logger.With(l => l.Trace(string.Format("Start ADTS test by channel {0}", _calibChan)));
            OnProgressChanged(new EventArgProgress(0, "Запуск Поверки"));
            if (!_adts.SetState(State.Control, cancel))
            {
                if(!cancel.IsCancellationRequested)
                    _logger.With(l => l.Trace(string.Format("[ERROR] set state {0}", State.Control)));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                whEnd.Set();
                OnEnd(new EventArgEnd(false));
                return;
            }

            OnResultUpdated(new EventArgTestResult(new ParameterDescriptor("CalibDate", null, ParameterType.Metadata), new ParameterResult(DateTime.Now, testDate)));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                whEnd.Set();
                OnEnd(new EventArgEnd(false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Поверка запущена (Дата: {0})", testDate.ToString())));
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
