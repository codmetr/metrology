using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using KipTM.Model.Devices;
using Tools;

namespace KipTM.Model.Checks
{
    public class ADTSCheckMethodic : ICheckMethodic
    {
        private ADTSModel _adts;
        private CancellationTokenSource _cancelSource;
        private NLog.Logger _logger;
        public ADTSCheckMethodic(ADTSModel adts, NLog.Logger logger)
        {
            _logger = logger;
            _adts = adts;
            _cancelSource = new CancellationTokenSource();
        }

        public IEnumerable<double> Points { get; set; }

        public PressureUnits Unit { get; set; }

        public double Rate { get; set; }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            _logger.With(l => l.Trace("Init ADTSCheckMethodic"));
            _adts.Init();
            _adts.StartAutoUpdate();
            return true;
        }

        /// <summary>
        /// Запуск калибровки
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="GetRealValue"></param>
        /// <param name="GetAccept"></param>
        /// <returns></returns>
        public bool Start(string channels, Func<double> GetRealValue, Func<bool> GetAccept )
        {
            var cancel = _cancelSource.Token;
            var countPoints = Points.Count();

            const double percentInitCalib = 5.0;
            const double percentEndPoints = 90.0;
            var percentOnePoint = (percentEndPoints - percentInitCalib)/countPoints;
            const double percentGetRes = 95.0;

            DateTime? calibDate;
            var calibChan = CalibChannel.PS;
            if(channels=="PT")
                calibChan = CalibChannel.PT;
            else if(channels == "PTPS")
                calibChan = CalibChannel.PS;
            else
                calibChan = CalibChannel.PS;
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            _logger.With(l => l.Trace(string.Format("Start ADTS calibration by channel \"{0}\"|{1}", channels, calibChan)));
            OnProgress(new CheckProgressEventArgs(0, "Калибровка запущена"));
            if (!_adts.StartCalibration(calibChan, out calibDate, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] start clibration")));
                OnError(new CheckErrorEventArgs() {Error = ADTSCheckError.ErrorStartCalibration});
                return false;
            }
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }

            int indexPoint = 0;
            Parameters param = calibChan == CalibChannel.PS ? Parameters.PS
                : calibChan == CalibChannel.PT ? Parameters.PT : Parameters.PS;
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
            foreach (var point in Points)
            {
                var percent = percentInitCalib + indexPoint*percentOnePoint;
                _logger.With( l => l.Trace(string.Format("Start calibration {0}, point[{4}//{5}] {1}; unit {2}; rate {3}",
                    param, point, Rate, Unit, indexPoint + 1, countPoints)));
                OnProgress(new CheckProgressEventArgs(percent, string.Format("Калибровка значения {0}", point)));
                if(_adts.SetPressure(param, point, Rate, Unit, cancel))
                {
                    _logger.With(l => l.Trace(string.Format("[ERROR] Set point")));
                    OnError(new CheckErrorEventArgs() { Error = ADTSCheckError.ErrorSetPressurePoint });
                    return false;
                }
                if (cancel.IsCancellationRequested)
                {
                    _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                    return false;
                }
                EventWaitHandle wh = param == Parameters.PT ? _adts.WaitPitotSetted() : _adts.WaitPressureSetted();

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
                var realValue = GetRealValue();
                _logger.With(l => l.Trace(string.Format("Real value {0}", realValue)));
                if (_adts.SetActualValue(realValue, cancel))
                {
                    OnError(new CheckErrorEventArgs() { Error = ADTSCheckError.ErrorSetRealValue });
                    return false;
                }

                if (cancel.IsCancellationRequested)
                {
                    _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                    return false;
                }
                indexPoint++;
            }

            double? slope;
            double? zero;

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            if (_adts.GetCalibrationResult(out slope, out zero, cancel))
            {
                OnError(new CheckErrorEventArgs() { Error = ADTSCheckError.ErrorGetResultCalibration });
                return false;
            }
            _logger.With(l => l.Trace(string.Format("Calibration result: slope {0}; zero: {1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));
            OnProgress(new CheckProgressEventArgs(percentGetRes, string.Format("Результат калибровки наклон:{0} ноль:{1}", (object)slope??"NULL", (object)zero??"NULL")));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            var accept = GetAccept();
            _logger.With(l => l.Trace(string.Format("Calibration accept: {0}", accept ? "accept" : "deny")));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            if (_adts.AcceptCalibration(accept, cancel))
            {
                OnError(new CheckErrorEventArgs() { Error = ADTSCheckError.ErrorAcceptResultCalibration });
                return false;
            }
            OnProgress(new CheckProgressEventArgs(100, string.Format("{0} результата калибровки", accept?"Подтверждение":"Отмена")));

            return true;
        }

        /// <summary>
        /// Отмена
        /// </summary>
        public void Cancel()
        {
            _cancelSource.Cancel();
        }

        /// <summary>
        /// Ошибка
        /// </summary>
        public EventHandler<CheckErrorEventArgs> Error;
        
        /// <summary>
        /// Изменился прогресс
        /// </summary>
        public EventHandler<CheckProgressEventArgs> Progress;

        #region Service methods
        protected virtual void OnError(CheckErrorEventArgs obj)
        {
            var handler = Error;
            if (handler != null) handler(this, obj);
        }

        protected virtual void OnProgress(CheckProgressEventArgs obj)
        {
            var handler = Progress;
            if (handler != null) handler(this, obj);
        }
        #endregion
    }
}
