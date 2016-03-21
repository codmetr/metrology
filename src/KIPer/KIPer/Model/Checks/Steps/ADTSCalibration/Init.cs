using System;
using System.Collections.Generic;
using System.Threading;
using ADTS;
using KipTM.Model.Devices;
using KipTM.Model.Params;
using NLog;
using Tools;

namespace KipTM.Model.Checks.Steps.ADTSCalibration
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

        public string Name { get; private set; }

        public override void Start(EventWaitHandle whEnd)
        {
            var cancel = _cancellationTokenSource.Token;
            DateTime? calibDate;
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
            }
            _logger.With(l => l.Trace(string.Format("Start ADTS calibration by channel {0}", _calibChan)));
            OnProgressChanged(new EventArgProgress(0, "Запуск калибровки"));
            if (!_adts.StartCalibration(_calibChan, out calibDate, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] start clibration")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                whEnd.Set();
                return;
            }
            if (calibDate!=null)
                OnResultUpdated(new EventArgTestResult(new ParameterDescriptor("CalibDate", null, ParameterType.Metadata), new ParameterResult(DateTime.Now, calibDate.Value)));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Калибровка запущена (Дата: {0})", calibDate == null ? "null" : calibDate.Value.ToString())));
            whEnd.Set();
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
