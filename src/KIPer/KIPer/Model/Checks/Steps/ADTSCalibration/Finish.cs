using System;
using System.Collections.Generic;
using System.Threading;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using KipTM.Model.Params;
using NLog;
using Tools;

namespace KipTM.Model.Checks.Steps.ADTSCalibration
{
    class Finish : TestStep
    {
        private readonly ADTSModel _adts;
        private readonly IUserChannel _userChannel;
        private readonly NLog.Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _checkCancelPeriod;

        public Finish(string name, ADTSModel adts, IUserChannel userChannel, Logger logger)
        {
            Name = name;
            _adts = adts;
            _logger = logger;
            _userChannel = userChannel;
            _cancellationTokenSource = new CancellationTokenSource();
            _checkCancelPeriod = TimeSpan.FromMilliseconds(10);
        }

        public override void Start(EventWaitHandle whEnd)
        {
            var cancel = _cancellationTokenSource.Token;
            double? slope;
            double? zero;

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
            }
            if (_adts.GetCalibrationResult(out slope, out zero, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] Can not get result calibration")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorGetResultCalibration });
                whEnd.Set();
                return;
            }
            _logger.With(l => l.Trace(string.Format("Calibration result: slope {0}; zero: {1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));
            OnResultUpdated(new EventArgTestResult(new ParameterDescriptor("Slope", null, ParameterType.RealValue),
                new ParameterResult(DateTime.Now, slope)));
            OnResultUpdated(new EventArgTestResult(new ParameterDescriptor("Zero", null, ParameterType.RealValue),
                new ParameterResult(DateTime.Now, zero)));
            //OnProgress(new EventArgCheckProgress(percentGetRes, string.Format("Результат калибровки наклон:{0} ноль:{1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
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
                whEnd.Set();
                return;
            }

            _logger.With(l => l.Trace(string.Format("Calibration accept: {0}", accept ? "accept" : "deny")));

            OnResultUpdated(new EventArgTestResult(new ParameterDescriptor("Accept", null, ParameterType.IsCorrect),
                new ParameterResult(DateTime.Now, accept)));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                whEnd.Set();
                return;
            }
            if (_adts.AcceptCalibration(accept, cancel))
            {
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorAcceptResultCalibration });
                whEnd.Set();
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("{0} результата калибровки", accept ? "Подтверждение" : "Отмена")));
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
