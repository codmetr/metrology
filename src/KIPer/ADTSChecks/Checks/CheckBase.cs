using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADTS;
using ADTSChecks.Model.Devices;
using ADTSData;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Model.Channels;
using CheckFrame.Model.Checks.Steps;
using KipTM.Archive;
using KipTM.EventAggregator;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel.Events;
using NLog;

namespace ADTSChecks.Model.Checks
{
    public abstract class CheckBase : ICheckMethod
    {
        public const string KeySettingsPS = KeysDic.KeySettingsPS;
        public const string KeySettingsPT = KeysDic.KeySettingsPT;
        public const string KeySettingsPSPT = KeysDic.KeySettingsPSPT;

        protected string MethodName = "ADTS";

        protected ADTSModel _adts;
        protected CancellationTokenSource _cancelSource;
        protected readonly NLog.Logger _logger;

        private IEventAggregator _agregator;
        protected ChannelDescriptor _calibChan;
        protected IEthalonChannel _ethalonChannel;
        protected IUserChannel _userChannel;

        private IEnumerable<CheckStepConfig> _steps;

        protected ITestStep _currenTestStep = null;
        protected readonly object _currenTestStepLocker = new object();

        public ITransportChannelType ChannelType;
        public ITransportChannelType EthalonChannelType;

        private IPausedStep _currentPause = null;
        private bool _isPauseAvailable;

        protected CheckBase(Logger logger)
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

        /// <summary>
        /// Указывает что доступность кнопки "Пауза" изменилась
        /// </summary>
        public event EventHandler PauseAvailableChanged;

        #endregion

        #region ICheckMethod
        /// <summary>
        /// Идентификатор методики
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Название методики
        /// </summary>
        public string Title{get { return MethodName; }}

        /// <summary>
        /// Инициализация 
        /// </summary>
        /// <returns></returns>
        public abstract bool Init(object customConf);

        public abstract object GetCustomConfig(IPropertyPool propertyPool);

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

            try
            {
                if (!_ethalonChannel.Activate(EthalonChannelType))
                    throw new Exception(string.Format("Can not Activate ethalon channel: {0}", _ethalonChannel));
            }
            catch (Exception e)
            {
                if(_agregator != null)
                    _agregator.Post(new ErrorMessageEventArg("Не удалось подключить эталонный канал"));
                OnEndMethod(null);
                return false;
            }
            foreach (var testStep in Steps)
            {
                whStep.Reset();
                var step = testStep;
                if(!step.Enabled)
                    continue;
                PrepareStartStep(step.Step);
                string error = string.Empty;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        step.Step.Start(whStep);
                    }
                    catch (Exception ex)
                    {
                        step.Step.Stop();
                        error = ex.Message;
                        whStep.Set();
                    }
                }, cancel);
                while (!whStep.WaitOne(waitPeriod))
                {
                    if (cancel.IsCancellationRequested)
                    {
                        testStep.Step.Stop();
                        break;
                    }
                }
                if (error != string.Empty)
                {
                    OnError(new EventArgError()
                    {
                        ErrorString = string.Format("На шаге \"{0}\" возникла ошибка: {1}", step.Step.Name, error)
                    });
                    break;
                }
                AfterEndStep(testStep.Step);
                if (cancel.IsCancellationRequested)
                    break;
            }
            SetCurrentPause(null);
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

        /// <summary>
        /// Ключ канала
        /// </summary>
        public string ChannelKey
        {
            get { return _calibChan.Name; }
        }

        /// <summary>
        /// Задать агрегатор событий
        /// </summary>
        /// <param name="agregator">агрегатор событий</param>
        public void SetAggregator(IEventAggregator agregator)
        {
            _agregator = agregator;
        }

        /// <summary>
        /// Задать канал эталона
        /// </summary>
        /// <param name="ethalonChannel"></param>
        /// <param name="transport"></param>
        public void SetEthalonChannel(IEthalonChannel ethalonChannel, ITransportChannelType transport)
        {
            _ethalonChannel = ethalonChannel;
            EthalonChannelType = transport;
            foreach (var testStep in Steps)
            {
                var step = testStep.Step as ISettedEthalonChannel;
                if(step==null)
                    continue;
                step.SetEthalonChannel(ethalonChannel);
            }
        }

        /// <summary>
        /// Задать канал связи с пользователем
        /// </summary>
        /// <param name="userChannel"></param>
        public void SetUserChannel(IUserChannel userChannel)
        {
            _userChannel = userChannel;
            foreach (var testStep in Steps)
            {
                var step = testStep.Step as ISettedUserChannel;
                if (step == null)
                    continue;
                step.SetUserChannel(userChannel);
            }
        }

        /// <summary>
        /// Задать АДТС
        /// </summary>
        /// <param name="adts"></param>
        public void SetADTS(ADTSModel adts)
        {
            _adts = adts;
        }

        /// <summary>
        /// Получить модель АДТС
        /// </summary>
        /// <returns></returns>
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
        /// Установить текущий шаг как объект паузы
        /// </summary>
        /// <param name="paused"></param>
        private void SetCurrentPause(IPausedStep paused)
        {
            if (_currentPause != null)
                _currentPause.PauseAccessibilityChanged -= CurrentPauseOnPauseAccessibilityChanged;

            _currentPause = paused;
            IsPauseAvailable = false;
            if (_currentPause == null)
                return;
            IsPauseAvailable = _currentPause.IsPauseAvailable;
            _currentPause.PauseAccessibilityChanged += CurrentPauseOnPauseAccessibilityChanged;
        }

        /// <summary>
        /// Обработка события изменения доступности паузы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CurrentPauseOnPauseAccessibilityChanged(object sender, EventArgs eventArgs)
        {
            if (_currentPause == null)
                return;
            IsPauseAvailable = _currentPause.IsPauseAvailable;
        }

        /// <summary>
        /// Доступность паузы
        /// </summary>
        public bool IsPauseAvailable
        {
            get { return _isPauseAvailable; }
            set { 
                if(_isPauseAvailable == value)
                    return;
                _isPauseAvailable = value; 
                OnPauseAvailableChanged();
            }
        }

        /// <summary>
        /// Поставить текущий шаг на паузу
        /// </summary>
        public void Pause()
        {
            if (_currentPause == null)
                return;

            _currentPause.Pause();
        }

        /// <summary>
        /// Возобновить текущий шаг с паузы
        /// </summary>
        public void Resume()
        {
            if (_currentPause == null)
                return;

            _currentPause.Resume();
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
                // Привязать обработку паузы, если она возможна для данного типа шага
                SetCurrentPause(step as IPausedStep);
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
                // Отвязать обработку паузы
                SetCurrentPause(null);
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

        protected virtual void OnPauseAvailableChanged()
        {
            EventHandler handler = PauseAvailableChanged;
            if (handler != null) handler(this, EventArgs.Empty);
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