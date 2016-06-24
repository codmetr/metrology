using System;
using System.Collections.Generic;
using System.Threading;
using ADTS;
using ArchiveData.DTO.Params;
using KipTM.Model.Devices;
using NLog;
using Tools;

namespace KipTM.Model.Checks.Steps.ADTSCalibration
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
            var cancel = _cancellationTokenSource.Token;
            DateTime? calibDate;
            OnStarted();
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            _logger.With(l => l.Trace(string.Format("Start ADTS calibration by channel {0}", _calibChan)));
            OnProgressChanged(new EventArgProgress(0, "Запуск калибровки"));
            if (!_adts.StartCalibration(_calibChan, out calibDate, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] start clibration")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                whEnd.Set();
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            if (calibDate!=null)
                OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyCalibDate, null, ParameterType.Metadata), new ParameterResult(DateTime.Now, calibDate.Value)));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Калибровка запущена (Дата: {0})", calibDate == null ? "null" : calibDate.Value.ToString())));
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
