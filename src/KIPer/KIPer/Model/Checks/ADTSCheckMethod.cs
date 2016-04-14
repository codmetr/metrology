using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using KipTM.Archive;
using KipTM.Model.Channels;
using KipTM.Model.Checks.Steps;
using KipTM.Model.Checks.Steps.ADTSCalibration;
using KipTM.Model.Devices;
using KipTM.Settings;
using Tools;

namespace KipTM.Model.Checks
{
    public class ADTSCheckMethod : ICheckMethod
    {
        public static string Key = "Калибровка ADTS";
        public const string KeySettingsPS = "ADTSCalibrationPs";
        public const string KeySettingsPT = "ADTSCalibrationPt";
        public const string KeySettingsPSPT = "ADTSCalibrationPsPt";

        public const string KeyPoints = "Points";
        public const string KeyRate = "Rate";
        public const string KeyUnit = "Unit";
        public const string KeyChannel = "Channel";

        private const string TitleMethod = "Калибровка ADTS";

        private ADTSModel _adts;
        private readonly CancellationTokenSource _cancelSource;
        private readonly NLog.Logger _logger;

        private CalibChannel _calibChan;
        private IEthalonChannel _ethalonChannel;
        private IUserChannel _userChannel;

        public ADTSCheckMethod(NLog.Logger logger)
        {
            _logger = logger;
            _cancelSource = new CancellationTokenSource();
        }

        public string Title{get { return TitleMethod; }}

        public IEnumerable<ADTSChechPoint> Points { get; set; }

        public CalibChannel Channel{get { return _calibChan; } set { _calibChan = value; }}

        public string ChannelKey
        {
            get
            {
                switch (_calibChan)
                {
                    case CalibChannel.PT:
                        return KeySettingsPS;
                    case CalibChannel.PS:
                        return KeySettingsPT;
                    case CalibChannel.PTPS:
                        return KeySettingsPSPT;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void SetEthalonChannel(IEthalonChannel ethalonChannel)
        {
            _ethalonChannel = ethalonChannel;
            foreach (var testStep in Steps)
            {
                var step = testStep as DoPoint;
                if(step==null)
                    continue;
                step.SetEthalonChannel(ethalonChannel);
            }
        }

        public void SetUserChannel(IUserChannel userChannel)
        {
            _userChannel = userChannel;
            foreach (var testStep in Steps)
            {
                var step = testStep as Finish;
                if (step == null)
                    continue;
                step.SetUserChannel(_userChannel);
            }
        }

        public void SetADTS(ADTSModel adts)
        {
            _adts = adts;
            
        }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public bool Init(IPropertyPool propertyes)
        {
            var points = propertyes.GetProperty<List<ADTSChechPoint>>(ADTSCheckMethod.KeyPoints);
            var channel = propertyes.GetProperty<CalibChannel>(ADTSCheckMethod.KeyChannel);
            var rate = propertyes.GetProperty<double>(ADTSCheckMethod.KeyRate);
            var unit = propertyes.GetProperty<PressureUnits>(ADTSCheckMethod.KeyUnit);
            return Init(new ADTSCheckParameters(channel, points, rate, unit));
        }

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public bool Init(ADTSCheckParameters parameters)
        {
            _logger.With(l => l.Trace("Init ADTSCheckMethodic"));

            _calibChan = parameters.CalibChannel;

            //if (_userChannel == null)
            //    throw new NullReferenceException("\"UserChannel\" not fount in parameters as IUserChannel");

            var steps = new List<ITestStep>();

            // добавление шага инициализации
            ITestStep step = new Init("Инициализация калибровки", _adts, _calibChan, _logger);
            steps.Add(step);
            step.ResultUpdated += step_ResultUpdated;

            // добавление шага прохождения точек
            Parameters param = _calibChan == CalibChannel.PS ? Parameters.PS
                : _calibChan == CalibChannel.PT ? Parameters.PT : Parameters.PS;
            foreach (var point in parameters.Points)
            {
                step = new DoPoint(string.Format("Калибровка точки {0}", point.Pressure), _adts, param, point.Pressure, point.Tolerance, parameters.Rate, parameters.Unit, _ethalonChannel, _logger);
                step.ResultUpdated += step_ResultUpdated;
                steps.Add(step);
            }

            // добавление шага подтверждения калибровки
            step = new Finish("Подтверждение калибровки", _adts, _userChannel, _logger);
            steps.Add(step);
            if (Steps != null)
                foreach (var testStep in Steps)
                {
                    if (testStep != null) testStep.ResultUpdated -= step_ResultUpdated;
                }
            Steps = steps;
            return true;
        }

        void step_ResultUpdated(object sender, EventArgTestResult e)
        {
            OnResultUpdated(e);
        }

        /// <summary>
        /// Запуск калибровки
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            var cancel = _cancelSource.Token;
            ManualResetEvent whStep = new ManualResetEvent(false);
            var waitPeriod = TimeSpan.FromMilliseconds(10);
            foreach (var testStep in Steps)
            {
                whStep.Reset();
                testStep.Start(whStep);
                while (!whStep.WaitOne(waitPeriod))
                {
                    if (cancel.IsCancellationRequested)
                    {
                        testStep.Stop();
                        break;
                    }
                }
                if (cancel.IsCancellationRequested)
                {
                    break;
                }
            }
            return true;

            /*
            var countPoints = Points.Count();

            const double percentInitCalib = 5.0;
            const double percentEndPoints = 90.0;
            var percentOnePoint = (percentEndPoints - percentInitCalib)/countPoints;
            const double percentGetRes = 95.0;

            DateTime? calibDate;
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            _logger.With(l => l.Trace(string.Format("Start ADTS calibration by channel {0}", _calibChan)));
            OnProgress(new EventArgProgress(0, "Калибровка запущена"));
            if (!_adts.StartCalibration(_calibChan, out calibDate, cancel))
            {
                _logger.With(l => l.Trace(string.Format("[ERROR] start clibration")));
                //OnError(new EventArgError() {Error = ADTSCheckError.ErrorStartCalibration});
                return false;
            }
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }

            int indexPoint = 0;
            Parameters param = _calibChan == CalibChannel.PS ? Parameters.PS
                : _calibChan == CalibChannel.PT ? Parameters.PT : Parameters.PS;
            TimeSpan waitPointPeriod = TimeSpan.FromMilliseconds(50);
            foreach (var point in Points)
            {
                var percent = percentInitCalib + indexPoint*percentOnePoint;
                _logger.With( l => l.Trace(string.Format("Start calibration {0}, point[{4}//{5}] {1}; unit {2}; rate {3}",
                    param, point.Pressure, Rate, Unit, indexPoint + 1, countPoints)));
                OnProgress(new EventArgProgress(percent, string.Format("Калибровка значения {0}", point.Pressure)));
                if(_adts.SetPressure(param, point.Pressure, Rate, Unit, cancel))
                {
                    _logger.With(l => l.Trace(string.Format("[ERROR] Set point")));
                    //OnError(new EventArgError() { Error = ADTSCheckError.ErrorSetPressurePoint });
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
                var realValue = _ethalonChannel.GetEthalonValue(point.Pressure, cancel);
                _logger.With(l => l.Trace(string.Format("Real value {0}", realValue)));
                if (_adts.SetActualValue(realValue, cancel))
                {
                    //OnError(new EventArgError() { Error = ADTSCheckError.ErrorSetRealValue });
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
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorGetResultCalibration });
                return false;
            }
            _logger.With(l => l.Trace(string.Format("Calibration result: slope {0}; zero: {1}", (object)slope ?? "NULL", (object)zero ?? "NULL")));
            OnProgress(new EventArgProgress(percentGetRes, string.Format("Результат калибровки наклон:{0} ноль:{1}", (object)slope??"NULL", (object)zero??"NULL")));

            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }

            _userChannel.Message = "Применить результаты калибровки?";
            var whAccept = new ManualResetEvent(false);
            TimeSpan waitAcceptPeriod = TimeSpan.FromMilliseconds(50);
            _userChannel.NeedQuery(UserQueryType.GetAccept, whAccept);
            while (whAccept.WaitOne(waitAcceptPeriod))
            {
                if (cancel.IsCancellationRequested)
                {
                    _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                    return false;
                }
            }
            var accept = _userChannel.AcceptValue;

            _logger.With(l => l.Trace(string.Format("Calibration accept: {0}", accept ? "accept" : "deny")));
            if (cancel.IsCancellationRequested)
            {
                _logger.With(l => l.Trace(string.Format("Cancel calibration")));
                return false;
            }
            if (_adts.AcceptCalibration(accept, cancel))
            {
                //OnError(new EventArgError() { Error = ADTSCheckError.ErrorAcceptResultCalibration });
                return false;
            }
            OnProgress(new EventArgProgress(100, string.Format("{0} результата калибровки", accept?"Подтверждение":"Отмена")));

            return true;*/
        }

        public IEnumerable<ITestStep> Steps
        {
            get { return _steps; }
            private set
            {
                _steps = value;
                OnStepsChanged();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
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
        public EventHandler<EventArgError> Error;
        
        /// <summary>
        /// Изменился прогресс
        /// </summary>
        public EventHandler<EventArgProgress> Progress;

        /// <summary>
        /// Изменился набор точек
        /// </summary>
        public EventHandler PointsChanged;

        /// <summary>
        /// Изменился набор шагов
        /// </summary>
        public EventHandler StepsChanged;

        /// <summary>
        /// Получен результат
        /// </summary>
        public EventHandler<EventArgTestResult> ResultUpdated;

        private IEnumerable<ITestStep> _steps;

        #region Service methods
        protected virtual void OnError(EventArgError obj)
        {
            var handler = Error;
            if (handler != null) handler(this, obj);
        }

        protected virtual void OnProgress(EventArgProgress obj)
        {
            var handler = Progress;
            if (handler != null) handler(this, obj);
        }

        protected virtual void OnPointsChanged()
        {
            var handler = PointsChanged;
            if (handler != null) handler(this, null);
        }

        protected virtual void OnStepsChanged()
        {
            var handler = StepsChanged;
            if (handler != null) handler(this, null);
        }

        protected virtual void OnResultUpdated(EventArgTestResult e)
        {
            var handler = ResultUpdated;
            if (handler != null) handler(this, e);
        }
        #endregion
    }
}
