using System;
using System.Threading;
using ADTS;
using ArchiveData.DTO.Params;
using KipTM.Model.Devices;
using NLog;
using Tools;

namespace KipTM.Model.Checks.Steps.ADTSTest
{
    class InitStep : TestStep
    {
        public const string KeyStep = "InitStep";
        public const string KeyCalibDate = "CalibDate";

        private readonly ADTSModel _adts;
        private readonly CalibChannel _calibChan;
        private readonly NLog.Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public InitStep(string name, ADTSModel adts, CalibChannel calibChan, Logger logger)
        {
            Name = name;
            _adts = adts;
            _calibChan = calibChan;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override void Start(EventWaitHandle whEnd)
        {
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
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
            _logger.With(l => l.Trace(string.Format("Start ADTS test by channel {0}", _calibChan)));
            OnProgressChanged(new EventArgProgress(0, "Запуск Поверки"));
            if (!_adts.SetState(State.Control, cancel))
            {
                if(!cancel.IsCancellationRequested)
                    _logger.With(l => l.Trace(string.Format("[ERROR] set state {0}", State.Control)));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                whEnd.Set();
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }

            // Дождаться установки параметра или примененения текущей точки как целевой
            EventWaitHandle wh = _adts.WaitControlSetted();
            while (!wh.WaitOne(waitPointPeriod))
            {
                if (cancel.IsCancellationRequested)
                {
                    _adts.StopWaitState(wh);
                    whEnd.Set();
                    OnEnd(new EventArgEnd(KeyStep, false));
                    return;
                }
            }

            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyCalibDate, null, ParameterType.Metadata), new ParameterResult(DateTime.Now, testDate)));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel test")));
                whEnd.Set();
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Поверка запущена (Дата: {0})", testDate.ToString())));
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
