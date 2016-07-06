using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using ArchiveData.DTO.Params;
using KipTM.Archive;
using KipTM.Model.Channels;
using KipTM.Model.Checks.Steps;
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

        protected string MethodName = "ADTS";

        protected ADTSModel _adts;
        protected CancellationTokenSource _cancelSource;
        protected readonly NLog.Logger _logger;

        protected CalibChannel _calibChan;
        protected IEthalonChannel _ethalonChannel;
        protected IUserChannel _userChannel;

        private IEnumerable<CheckStepConfig> _steps;

        protected ITestStep _currenTestStep = null;
        protected readonly object _currenTestStepLocker = new object();

        public ITransportChannelType ChannelType;
        public ITransportChannelType EthalonChannelType;

        protected ADTSMethodBase(Logger logger)
        {
            _logger = logger;
            _cancelSource = new CancellationTokenSource();
        }

        #region events
        /// <summary>
        /// Ошибка
        /// </summary>
        public event EventHandler<EventArgError> Error;

        /// <summary>
        /// Изменился прогресс
        /// </summary>
        public event EventHandler<EventArgProgress> Progress;

        /// <summary>
        /// Изменился набор точек
        /// </summary>
        public event EventHandler PointsChanged;

        /// <summary>
        /// Изменился набор шагов
        /// </summary>
        public event EventHandler StepsChanged;

        /// <summary>
        /// Получен результат
        /// </summary>
        public event EventHandler<EventArgTestStepResult> ResultUpdated;

        /// <summary>
        /// Проход закончен
        /// </summary>
        public event EventHandler EndMethod;
        #endregion

        #region ICheckMethod
        
        /// <summary>
        /// Название методики
        /// </summary>
        public string Title{get { return MethodName; }}

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public abstract bool Init(IPropertyPool propertyes);

        /// <summary>
        /// Запуск методики
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            _adts.Start(ChannelType); 
            var cancel = _cancelSource.Token;
            var whStep = new ManualResetEvent(false);
            var waitPeriod = TimeSpan.FromMilliseconds(10);
            if (!_ethalonChannel.Activate(EthalonChannelType))
                throw new Exception(string.Format("Can not Activate ethalon channel: {0}", _ethalonChannel));
            foreach (var testStep in Steps)
            {
                whStep.Reset();
                PrepareStartStep(testStep.Step);
                var step = testStep;
                if(!step.Enabled)
                    continue;
                Task.Factory.StartNew(() => step.Step.Start(whStep), cancel);
                while (!whStep.WaitOne(waitPeriod))
                {
                    if (cancel.IsCancellationRequested)
                    {
                        testStep.Step.Stop();
                        break;
                    }
                }
                AfterEndStep(testStep.Step);
                if (cancel.IsCancellationRequested)
                {
                    break;
                }
            }
            _ethalonChannel.Stop();

            OnEndMethod(null);
            return true;

        }

        /// <summary>
        /// Остановка методики
        /// </summary>
        public void Stop()
        {
            if (Steps != null)
                foreach (var testStep in Steps)
                {
                    if (testStep != null) testStep.Step.ResultUpdated -= StepResultUpdated;
                }
            Cancel(); 
            ToBaseAction();
        }

        /// <summary>
        /// Список шагов
        /// </summary>
        public IEnumerable<CheckStepConfig> Steps
        {
            get { return _steps; }
            protected set
            {
                _steps = value;
                OnStepsChanged();
            }
        }
        #endregion

        public string ChannelKey
        {
            get
            {
                switch (_calibChan)
                {
                    case CalibChannel.PT:
                        return KeySettingsPT;
                    case CalibChannel.PS:
                        return KeySettingsPS;
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
                var step = testStep as ISettedEthalonChannel;
                if(step==null)
                    continue;
                step.SetEthalonChannel(ethalonChannel);
            }
        }

        public void SetADTS(ADTSModel adts)
        {
            _adts = adts;
        }

        public ADTSModel GetADTS()
        {
            return _adts;
        }

        /// <summary>
        /// Установить текущую точку как очередную ожидаемую
        /// </summary>
        public void SetCurrentValueAsPoint()
        {
            IStoppedOnPoint pointstep;
            lock (_currenTestStepLocker)
            {
                pointstep = _currenTestStep as IStoppedOnPoint;
            }
            if (pointstep == null)
                return;
            pointstep.SetCurrentValueAsPoint();
        }

        /// <summary>
        /// Отмена
        /// </summary>
        public void Cancel()
        {
            _cancelSource.Cancel();
            _cancelSource = new CancellationTokenSource();
        }

        #region Steps events
        /// <summary>
        /// Вызывается перед запуском шага
        /// </summary>
        /// <param name="step"></param>
        protected virtual void PrepareStartStep(ITestStep step)
        {
            lock (_currenTestStepLocker)
            {
                _currenTestStep = step;
            }
        }

        /// <summary>
        /// Вызывается после завершения шага
        /// </summary>
        /// <param name="step"></param>
        protected virtual void AfterEndStep(ITestStep step)
        {
            lock (_currenTestStepLocker)
            {
                _currenTestStep = null;
            }
        }

        protected void AttachStep(ITestStep step)
        {
            step.ResultUpdated += StepResultUpdated;
            step.End += StepEnd;
        }

        protected void DetachStep(ITestStep step)
        {
            step.ResultUpdated -= StepResultUpdated;
            step.End -= StepEnd;
        }

        protected abstract void StepEnd(object sender, EventArgEnd e);

        protected virtual void StepResultUpdated(object sender, EventArgStepResult e)
        {
            FillResult(e);
        }
        #endregion

        #region Event invocators

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

        protected virtual void OnResultUpdated(EventArgTestStepResult e)
        {
            EventHandler<EventArgTestStepResult> handler = ResultUpdated;
            if (handler != null) handler(this, e);
        }
 
        protected virtual void OnEndMethod(EventArgs e)
        {
            var handler = EndMethod;
            if (handler != null) handler(this, e);
        }
        #endregion

        #region Fill results
        /// <summary>
        /// Заполнение полученных результатов проверки
        /// </summary>
        /// <param name="e"></param>
        private void FillResult(EventArgStepResult e)
        {
            foreach (var parameterResult in e.Result)
            {
                SwitchParameter(parameterResult.Key, parameterResult.Value);
            }
        }

        /// <summary>
        /// Распределить результат в нужное поле результата
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="result"></param>
        protected abstract void SwitchParameter(ParameterDescriptor descriptor, ParameterResult result);

        #endregion

        protected virtual void ToBaseAction()
        {
            var whStep = new ManualResetEvent(false);
            var end = Steps.FirstOrDefault(el => el.Step is IToBaseStep);
            if (end != null)
            {
                whStep.Reset();
                end.Step.Start(whStep);
            }
        }
    }
}