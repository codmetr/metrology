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
using KipTM.Model.Checks.Steps.ADTSTest;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Settings;
using Tools;

namespace KipTM.Model.Checks
{
    public class ADTSTestMethod : ICheckMethod
    {
        public static string Key = "Поверка ADTS";
        public const string KeySettingsPS = "ADTSCalibrationPs";
        public const string KeySettingsPT = "ADTSCalibrationPt";
        public const string KeySettingsPSPT = "ADTSCalibrationPsPt";

        public const string KeyPoints = "Points";
        public const string KeyRate = "Rate";
        public const string KeyUnit = "Unit";
        public const string KeyChannel = "Channel";

        private const string TitleMethod = "Поверка ADTS";

        private ADTSModel _adts;
        private  CancellationTokenSource _cancelSource;
        private readonly NLog.Logger _logger;

        private CalibChannel _calibChan;
        private IEthalonChannel _ethalonChannel;

        public ADTSTestMethod(NLog.Logger logger)
        {
            _logger = logger;
            _cancelSource = new CancellationTokenSource();
        }

        public int Address;

        public ITransportChannelType ChannelType;

        public ITransportChannelType EthalonChannelType;

        public string Title { get { return TitleMethod; } }

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

        public void SetEthalonChannel(IEthalonChannel ethalonChannel, ITransportChannelType transport)
        {
            _ethalonChannel = ethalonChannel;
            EthalonChannelType = transport;
            foreach (var testStep in Steps)
            {
                var step = testStep as DoPoint;
                if(step==null)
                    continue;
                step.SetEthalonChannel(ethalonChannel);
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
            ITestStep step = new Init("Инициализация поверки", _adts, _calibChan, _logger);
            steps.Add(step);
            step.ResultUpdated += step_ResultUpdated;

            // добавление шага прохождения точек
            Parameters param = _calibChan == CalibChannel.PS ? Parameters.PS
                : _calibChan == CalibChannel.PT ? Parameters.PT : Parameters.PS;
            foreach (var point in parameters.Points)
            {
                step = new DoPoint(string.Format("Поверка точки {0}", point.Pressure), _adts, param, point.Pressure, point.Tolerance, parameters.Rate, parameters.Unit, _ethalonChannel, _logger);
                step.ResultUpdated += step_ResultUpdated;
                steps.Add(step);
            }

            if (Steps != null)
                foreach (var testStep in Steps)
                {
                    if (testStep != null)
                        testStep.ResultUpdated -= step_ResultUpdated;
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
            _adts.Start(ChannelType);
            var cancel = _cancelSource.Token;
            ManualResetEvent whStep = new ManualResetEvent(false);
            var waitPeriod = TimeSpan.FromMilliseconds(10);
            if (!_ethalonChannel.Activate(EthalonChannelType))
                throw new Exception(string.Format("Can not Activate ethalon channel: {0}", _ethalonChannel));
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
            _ethalonChannel.Stop();
            return true;

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
            if (Steps != null)
                foreach (var testStep in Steps)
                {
                    if (testStep != null) testStep.ResultUpdated -= step_ResultUpdated;
                }
            _cancelSource.Cancel();
        }

        /// <summary>
        /// Отмена
        /// </summary>
        public void Cancel()
        {
            _cancelSource.Cancel();
            _cancelSource = new CancellationTokenSource();
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
