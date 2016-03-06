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
    class ADTSCalibrationPoint:ITestStep
    {
        private readonly ADTSModel _adts;
        private readonly Parameters _param;
        private readonly double _point;
        private readonly double _rate;
        private readonly PressureUnits _unit;
        private readonly Func<double> _getRealValue;
        private readonly NLog.Logger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ADTSCalibrationPoint(string name, ADTSModel adts, Parameters param, double point, double rate, PressureUnits unit, Func<double> getRealValue, Logger logger)
        {
            Name = name;
            _adts = adts;
            _param = param;
            _point = point;
            _rate = rate;
            _unit = unit;
            _logger = logger;
            _getRealValue = getRealValue;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public string Name { get; private set; }

        public bool Run()
        {
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
            var cancel = _cancellationTokenSource.Token;
            //var percent = percentInitCalib + indexPoint * percentOnePoint;
            //_logger.With(l => l.Trace(string.Format("Start calibration {0}, point[{4}//{5}] {1}; unit {2}; rate {3}",
            //    _param, point, Rate, Unit, indexPoint + 1, countPoints)));
            //OnProgressChanged(new EventArgCheckProgress(percent, string.Format("Калибровка значения {0}", point)));
            if (_adts.SetPressure(_param, _point, _rate, _unit, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] Set point")));
                OnError(new EventArgError() { Error = ADTSCheckError.ErrorSetPressurePoint });
                return false;
            }
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            EventWaitHandle wh = _param == Parameters.PT ? _adts.WaitPitotSetted() : _adts.WaitPressureSetted();

            while (wh.WaitOne(waitPointPeriod))
            {
                if (cancel.IsCancellationRequested)
                {
                    _adts.StopWaitStatus(wh);
                    return false;
                }
            }

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            var realValue = _getRealValue();
            _logger.With(l => l.Trace(string.Format("Real value {0}", realValue)));
            if (_adts.SetActualValue(realValue, cancel))
            {
                OnError(new EventArgError() { Error = ADTSCheckError.ErrorSetRealValue });
                return false;
            }

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
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
