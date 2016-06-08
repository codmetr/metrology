using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using KipTM.Archive;
using KipTM.Model.Channels;
using KipTM.Model.Checks.Steps.ADTSCalibration;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using NLog;

namespace KipTM.Model.Checks
{
    public abstract class ADTSMethodBase : ICheckMethod
    {
        public const string KeySettingsPS = "ADTSCalibrationPs";
        public const string KeySettingsPT = "ADTSCalibrationPt";
        public const string KeySettingsPSPT = "ADTSCalibrationPsPt";

        protected ADTSModel _adts;
        private CancellationTokenSource _cancelSource;
        protected readonly NLog.Logger _logger;
        protected CalibChannel _calibChan;
        protected IEthalonChannel _ethalonChannel;
        protected IUserChannel _userChannel;
        public ITransportChannelType ChannelType;
        public ITransportChannelType EthalonChannelType;

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

        protected string MethodName = "Калибровка ADTS";

        protected ADTSMethodBase(Logger logger)
        {
            _logger = logger;
            _cancelSource = new CancellationTokenSource();
        }

        public string Title{get { return MethodName; }}
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

        public IEnumerable<ITestStep> Steps
        {
            get { return _steps; }
            protected set
            {
                _steps = value;
                OnStepsChanged();
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
        public abstract bool Init(IPropertyPool propertyes);

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public abstract bool Init(ADTSMethodParameters parameters);

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
                PrepareStartStep(testStep);
                var step = testStep;
                Task.Factory.StartNew(() => step.Start(whStep), cancel);
                while (!whStep.WaitOne(waitPeriod))
                {
                    if (cancel.IsCancellationRequested)
                    {
                        testStep.Stop();
                        break;
                    }
                }
                AfterEndStep(testStep);
                if (cancel.IsCancellationRequested)
                {
                    break;
                }
            }
            return true;

        }

        public void Stop()
        {
            if (Steps != null)
                foreach (var testStep in Steps)
                {
                    if (testStep != null) testStep.ResultUpdated -= StepResultUpdated;
                }
            _cancelSource.Cancel();
            _cancelSource = new CancellationTokenSource();
            ToBaseAction();
        }

        /// <summary>
        /// Отмена
        /// </summary>
        public void Cancel()
        {
            _cancelSource.Cancel();
        }

        /// <summary>
        /// Вызывается перед запуском шага
        /// </summary>
        /// <param name="step"></param>
        protected virtual void PrepareStartStep(ITestStep step)
        {
            
        }

        /// <summary>
        /// Вызывается после завершения шага
        /// </summary>
        /// <param name="step"></param>
        protected virtual void AfterEndStep(ITestStep step)
        {

        }

        protected void StepResultUpdated(object sender, EventArgTestResult e)
        {
            OnResultUpdated(e);
        }

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


        protected virtual void ToBaseAction()
        {
        }
    }
}