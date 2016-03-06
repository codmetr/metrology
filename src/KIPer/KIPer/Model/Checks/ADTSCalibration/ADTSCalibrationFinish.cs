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
    class ADTSCalibrationFinish:ITestStep
    {
        private readonly ADTSModel _adts;
        private readonly Func<bool> _getAccept;
        private readonly NLog.Logger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ADTSCalibrationFinish(string name, ADTSModel adts, Func<bool> getAccept, Logger logger)
        {
            Name = name;
            _adts = adts;
            _logger = logger;
            _getAccept = getAccept;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public string Name { get; private set; }

        public bool Run()
        {
            var cancel = _cancellationTokenSource.Token;
            double? slope;
            double? zero;

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            if (_adts.GetCalibrationResult(out slope, out zero, cancel))
            {
                OnError(new EventArgError() { Error = ADTSCheckError.ErrorGetResultCalibration });
                return false;
            }
            _logger.With(l => l.Trace(string.Format("Calibration result: slope {0}; zero: {1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));
            //OnProgress(new EventArgCheckProgress(percentGetRes, string.Format("Результат калибровки наклон:{0} ноль:{1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            var accept = _getAccept();
            _logger.With(l => l.Trace(string.Format("Calibration accept: {0}", accept ? "accept" : "deny")));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            if (_adts.AcceptCalibration(accept, cancel))
            {
                OnError(new EventArgError() { Error = ADTSCheckError.ErrorAcceptResultCalibration });
                return false;
            }
            OnProgressChanged(new EventArgCheckProgress(100, string.Format("{0} результата калибровки", accept ? "Подтверждение" : "Отмена")));
            return true;
        }

        public bool Stop()
        {
            _cancellationTokenSource.Cancel();
            return true;
        }

        public event EventHandler<EventArgCheckProgress> ProgressChanged;

        protected virtual void OnProgressChanged(EventArgCheckProgress e)
        {
            EventHandler<EventArgCheckProgress> handler = ProgressChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<EventArgError> Error;

        protected virtual void OnError(EventArgError e)
        {
            EventHandler<EventArgError> handler = Error;
            if (handler != null) handler(this, e);
        }
    }
}
