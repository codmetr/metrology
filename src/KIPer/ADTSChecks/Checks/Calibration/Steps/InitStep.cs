using System;
using System.Threading;
using ADTS;
using ADTSChecks.Devices;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Checks.Steps;
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

        public InitStep(string name, ADTSModel adts, ChannelDescriptor calibChan, Logger logger)
        {
            Name = name;
            _adts = adts;
            _calibChan = calibChan;
            _logger = logger;
        }

        public override void Start(CancellationToken cancel)
        {
            DateTime? calibDate;
            OnStarted();
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            _logger.With(l => l.Trace(string.Format("Start ADTS calibration by channel {0}", _calibChan.Name)));
            OnProgressChanged(new EventArgProgress(0, "Запуск калибровки"));
            if (!_adts.StartCalibration(_calibChan.Name, out calibDate, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] start clibration")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            if (calibDate!=null)
                OnResultUpdated(new EventArgStepResult(new ParameterDescriptor(KeyCalibDate, null, ParameterType.Metadata), new ParameterResult(DateTime.Now, calibDate.Value)));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("Калибровка запущена (Дата: {0})", calibDate == null ? "null" : calibDate.Value.ToString())));
            OnEnd(new EventArgEnd(KeyStep, true));
            return;
        }
    }
}
