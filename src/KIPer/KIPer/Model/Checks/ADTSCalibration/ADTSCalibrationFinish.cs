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
        private readonly IUserChannel _userChannel;
        private readonly NLog.Logger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _checkCancelPeriod;

        public ADTSCalibrationFinish(string name, ADTSModel adts, IUserChannel userChannel, Logger logger)
        {
            Name = name;
            _adts = adts;
            _logger = logger;
            _userChannel = userChannel;
            _cancellationTokenSource = new CancellationTokenSource();
            _checkCancelPeriod = TimeSpan.FromMilliseconds(10);
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
                _logger.With(l => l.Trace(string.Format("[ERROR] Can not get result calibration")));
                OnError(new EventArgError() { Error = ADTSCheckError.ErrorGetResultCalibration });
                return false;
            }
            _logger.With(l => l.Trace(string.Format("Calibration result: slope {0}; zero: {1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));
            OnResultsAdded(new EventArgResultParam(new Dictionary<string, object>()
            {
                { string.Format("Calibration results: Slope"), slope },
                { string.Format("Calibration results: Zero"), zero },
            }));
            //OnProgress(new EventArgCheckProgress(percentGetRes, string.Format("Результат калибровки наклон:{0} ноль:{1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            _userChannel.Message = string.Format("Применить результат калибровки?");//TODO: локализовать
            var wh = new AutoResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetRealValue, wh);
            while (!wh.WaitOne(_checkCancelPeriod))
            {
                if (cancel.IsCancellationRequested)
                    break;
            }
            bool accept = _userChannel.AcceptValue;
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }

            _logger.With(l => l.Trace(string.Format("Calibration accept: {0}", accept ? "accept" : "deny")));

            OnResultsAdded(new EventArgResultParam(new Dictionary<string, object>()
            {
                { string.Format("Calibration accept"), accept },
            }));
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
            OnProgressChanged(new EventArgCheckProgress(100,
                string.Format("{0} результата калибровки", accept ? "Подтверждение" : "Отмена")));
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
