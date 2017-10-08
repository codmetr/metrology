using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ArchiveData.DTO;
using ArchiveData.DTO.Params;
using CheckFrame.Model.Checks.Steps;
using KipTM.Archive;
using KipTM.EventAggregator;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Checks;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Checks.Steps;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel.Events;
using NLog;

namespace CheckFrame.Checks
{
    /// <summary>
    /// Базовая реализация проверки с линейным прохождением шагов
    /// </summary>
    public abstract class CheckBase:ICheckMethod
    {
        protected readonly Logger _logger;

        private IEventAggregator _agregator;
        protected ChannelDescriptor _calibChan;
        protected IEthalonChannel _ethalonChannel;
        protected IUserChannel _userChannel;

        private IEnumerable<CheckStepConfig> _steps;

        protected ITestStep _currenTestStep = null;
        protected readonly object _currenTestStepLocker = new object();

        /// <summary>
        /// Описатель канала подключения к целевому устройству
        /// </summary>
        public ITransportChannelType ChannelType;
        /// <summary>
        /// Описатель канала подключения к эталонному устройству
        /// </summary>
        public ITransportChannelType EthalonChannelType;

        private IPausedStep _currentPause = null;
        private bool _isPauseAvailable;

        protected CheckBase(Logger logger)
        {
            _logger = logger;
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
        public string Title { get; set; }

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
        public bool Start(CancellationToken cancel)
        {
            OnStartAction(cancel);

            try
            {
                if (!_ethalonChannel.Activate(EthalonChannelType))
                    throw new Exception(string.Format("Can not Activate ethalon channel: {0}", _ethalonChannel));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (_agregator != null)
                    _agregator.Post(new ErrorMessageEventArg("Не удалось подключить эталонный канал"));
                OnEndMethod(null);
                return false;
            }
            try
            {
                foreach (var testStep in Steps)
                {
                    var step = testStep;
                    if (!step.Enabled)
                        continue;
                    PrepareStartStep(step.Step);
                    string error = string.Empty;
                    try
                    {
                        step.Step.Start(cancel);
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
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
            }
            finally
            {
                SetCurrentPause(null);
                ToBaseAction();
                _ethalonChannel.Stop();
                OnEndMethod(null);
            }
            return true;

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
        /// Деcтвие перед запуском проверки
        /// </summary>
        protected virtual void OnStartAction(CancellationToken cancel)
        {
        }

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
                if (step == null)
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
        /// Установить текущий шаг как объект паузы
        /// </summary>
        /// <param name="paused"></param>
        private void SetCurrentPause(IPausedStep paused)
        {
            if (_currentPause != null)
                _currentPause.PauseAccessibilityChanged -= OnPauseAccessibilityChanged;

            _currentPause = paused;
            IsPauseAvailable = false;
            if (_currentPause == null)
                return;
            IsPauseAvailable = _currentPause.IsPauseAvailable;
            _currentPause.PauseAccessibilityChanged += OnPauseAccessibilityChanged;
        }

        /// <summary>
        /// Обработка события изменения доступности паузы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnPauseAccessibilityChanged(object sender, EventArgs eventArgs)
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
            set
            {
                if (_isPauseAvailable == value)
                    return;
                _isPauseAvailable = value;
                OnPauseAvailableChanged();
            }
        }

        /// <summary>
        /// Поставить текущий шаг на паузу
        /// </summary>
        public void Pause(CancellationToken cancel)
        {
            if (_currentPause == null)
                return;

            _currentPause.Pause(cancel);
        }

        /// <summary>
        /// Возобновить текущий шаг с паузы
        /// </summary>
        public void Resume(CancellationToken cancel)
        {
            if (_currentPause == null)
                return;

            _currentPause.Resume(cancel);
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

        protected virtual void AttachStep(ITestStep step)
        {
            step.End += StepEnd;
        }

        protected virtual void DetachStep(ITestStep step)
        {
            step.End -= StepEnd;
        }

        protected abstract void StepEnd(object sender, EventArgEnd e);

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

        protected virtual void ToBaseAction()
        {
            var whStep = new ManualResetEvent(false);
            var end = Steps.FirstOrDefault(el => el.Step is IToBaseStep);
            if (end != null)
            {
                whStep.Reset();
                end.Step.Start(CancellationToken.None);
            }
        }
    }
}
