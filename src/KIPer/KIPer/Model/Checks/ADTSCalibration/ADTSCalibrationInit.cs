using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using KipTM.Model.Devices;
using NLog;
using Tools;

namespace KipTM.Model.Checks.ADTSCalibration
{
    class ADTSCalibrationInit:ITestStep
    {
        private readonly ADTSModel _adts;
        private readonly CalibChannel _calibChan;
        private readonly NLog.Logger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ADTSCalibrationInit(string name, ADTSModel adts, CalibChannel calibChan, Logger logger)
        {
            Name = name;
            _adts = adts;
            _calibChan = calibChan;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public string Name { get; private set; }

        public bool Run()
        {
            var cancel = _cancellationTokenSource.Token;
            DateTime? calibDate;
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            _logger.With(l => l.Trace(string.Format("Start ADTS calibration by channel {0}", _calibChan)));
            OnProgressChanged(new EventArgCheckProgress(0, "Запуск калибровки"));
            if (!_adts.StartCalibration(_calibChan, out calibDate, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] start clibration")));
                OnError(new EventArgError() { Error = ADTSCheckError.ErrorStartCalibration });
                return false;
            }
            if (calibDate!=null)
                OnResultsAdded(new EventArgResultParam(new Dictionary<string, object>()
                {
                    {"Calibration date", calibDate.Value}
                }));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            OnProgressChanged(new EventArgCheckProgress(100,
                string.Format("Калибровка запущена (Дата: {0})", calibDate == null ? "null" : calibDate.Value.ToString())));
            return true;
        }

        public bool Stop()
        {
            _cancellationTokenSource.Cancel();
            return true;
        }

        public event EventHandler<EventArgResultParam> ResultsAdded;

        public event EventHandler<EventArgCheckProgress> ProgressChanged;

        public event EventHandler<EventArgError> Error;

        protected virtual void OnResultsAdded(EventArgResultParam e)
        {
            EventHandler<EventArgResultParam> handler = ResultsAdded;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnProgressChanged(EventArgCheckProgress e)
        {
            EventHandler<EventArgCheckProgress> handler = ProgressChanged;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnError(EventArgError e)
        {
            EventHandler<EventArgError> handler = Error;
            if (handler != null) handler(this, e);
        }
    }
}
