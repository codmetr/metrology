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

        protected ChannelsConfig _channels = new ChannelsConfig();

        private IEnumerable<CheckStepConfig> _steps;

        protected ITestStep _currenTestStep = null;
        protected readonly object _currenTestStepLocker = new object();

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
            if (!PrepareCheck(cancel))
            {
                OnEndMethod(null);
                return false;
            }
            ITestStep currentStep = null;
            try
            {
                foreach (var testStep in Steps)
                {
                    var step = testStep;
                    if (step == null || step.Step == null)
                    {
                        Log(string.Format("[WARNING] Step is Null"));
                        continue;
                    }
                    currentStep = step.Step;
                    if (step.Step is IFinalizeStep)
                        continue;
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
                    AfterEndStep(step.Step);
                    if (cancel.IsCancellationRequested)
                        break;
                }
            }
            finally
            {
                SetCurrentPause(null);
                //Инициативный запуск финализирующих шагов
                try
                {
                    Log("Runing finalizing step");
                    foreach (var step in Steps)
                    {
                        if (step == null || step.Step == null)
                        {
                            Log(string.Format("[WARNING] Step is Null"));
                            continue;
                        }
                        if (!(step.Step is IFinalizeStep))
                            continue;
                        currentStep = step.Step;
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
                                ErrorString = string.Format("На шаге \"{0}\" возникла ошибка: {1}", step.Step.Name, error) // TODO Локализовать
                            });
                            break;
                        }
                        AfterEndStep(step.Step);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    OnError(new EventArgError()
                        {
                            ErrorString = String.Format("Шаг \"{0}\" окончен по ошибке: {1}", currentStep.Name, ex.Message) // TODO Локализовать
                    });
                    Log(string.Format("On time step {0} throwed {1}", currentStep.Name, ex.ToString()));
                    throw;
                }
                finally
                {
                    Log("Finalizing steps End");
                }
                ToBaseAction();
                EndCheck();
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
        protected virtual bool PrepareCheck(CancellationToken cancel)
        {
            try
            {
                ChConfig.Activate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Деcтвие после проверки
        /// </summary>
        protected virtual void EndCheck()
        {
            ChConfig.Stop();
            OnEndMethod(null);
        }

        /// <summary>
        /// Конфигурация каналов
        /// </summary>
        public ChannelsConfig ChConfig { get { return _channels; } }

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

        protected void Log(string msg)
        {
            _logger.Trace(msg);
        }
    }
}
