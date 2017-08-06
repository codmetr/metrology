using System;
using System.Threading;
using ADTSChecks.Model.Devices;
using ArchiveData.DTO.Params;
using CheckFrame.Checks.Steps;
using CheckFrame.Model.Channels;
using CheckFrame.Model.Checks.Steps;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using NLog;
using Tools;

namespace ADTSChecks.Model.Steps.ADTSCalibration
{
    class FinishStep : TestStep, ISettedUserChannel
    {
        public const string KeyStep = "FinishStep";
        private readonly ADTSModel _adts;
        private IUserChannel _userChannel;
        private readonly NLog.Logger _logger;

        public FinishStep(string name, ADTSModel adts, IUserChannel userChannel, Logger logger)
        {
            Name = name;
            _adts = adts;
            _logger = logger;
            _userChannel = userChannel;
        }

        public override void Start(CancellationToken cancel)
        {
            double? slope;
            double? zero;
            OnStarted();
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            if (_adts.GetCalibrationResult(out slope, out zero, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] Can not get result calibration")));
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorGetResultCalibration });
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            _logger.With(l => l.Trace(string.Format("Calibration result: slope {0}; zero: {1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor("Slope", null, ParameterType.RealValue),
                new ParameterResult(DateTime.Now, slope)));
            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor("Zero", null, ParameterType.RealValue),
                new ParameterResult(DateTime.Now, zero)));
            //OnProgress(new EventArgCheckProgress(percentGetRes, string.Format("Результат калибровки наклон:{0} ноль:{1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            _userChannel.Message =
                string.Format("Что бы применить результат калибровки нажмите \"Подтвердить\", в противном случае нажмите ") +
                "\"{0}\"";//string.Format("Применить результат калибровки?");//TODO: локализовать
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);

            WaitHandle.WaitAny(new[] {wh, cancel.WaitHandle});
            bool accept = _userChannel.AcceptValue;
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }

            _logger.With(l => l.Trace(string.Format("Calibration accept: {0}", accept ? "accept" : "deny")));

            OnResultUpdated(new EventArgStepResult(new ParameterDescriptor("Accept", null, ParameterType.IsCorrect),
                new ParameterResult(DateTime.Now, accept)));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            if (_adts.AcceptCalibration(accept, cancel))
            {
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorAcceptResultCalibration });
                OnEnd(new EventArgEnd(KeyStep, false));
                return;
            }
            OnProgressChanged(new EventArgProgress(100,
                string.Format("{0} результата калибровки", accept ? "Подтверждение" : "Отмена")));
            OnEnd(new EventArgEnd(KeyStep, true));
            return;
        }

        public void SetUserChannel(IUserChannel userChannel)
        {
            _userChannel = userChannel;
        }
    }
}
