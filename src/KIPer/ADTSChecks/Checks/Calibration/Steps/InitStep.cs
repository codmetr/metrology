using System;
using System.Threading;
using ADTS;
using ADTSChecks.Model.Devices;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Model.Checks.Steps;
using KipTM.Model.Checks;
using NLog;
using Tools;

namespace ADTSChecks.Model.Steps.ADTSCalibration
{
    class InitStep : TestStep
    {
        public const string KeyStep = "InitStep";
        public const string KeyCalibDate = "CalibDate";

        private readonly ADTSModel _adts;
        private readonly ChannelDescriptor _calibChan;
        private readonly NLog.Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public InitStep(string name, ADTSModel adts, ChannelDescriptor calibChan, Logger logger)
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
            _logger.With(l => l.Trace(string.Format("Start ADTS calibration by channel {0}", _calibChan.Name)));
            OnProgressChanged(new EventArgProgress(0, "Запуск калибровки"));
            if (!_adts.StartCalibration(_calibChan.Name, out calibDate, cancel))
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
