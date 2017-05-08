using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using ADTSChecks.Checks.Data;
using ADTSChecks.Model.Checks;
using ADTSChecks.ViewModel.Services;
using ADTSData;
using ArchiveData.DTO;
using CheckFrame.Model.Channels;
using CheckFrame.ViewModel.Checks;
using CheckFrame.ViewModel.Checks.Channels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.EventAggregator;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel.Events;
using Tools.View;

namespace ADTSChecks.Checks.ViewModel
{
    /// <summary>
    /// Базовый класс визуальной модели проверки
    /// </summary>
    public abstract class CheckBaseViewModel : ViewModelBase, IMethodViewModel
    {
        #region Members

        private IEventAggregator _agregator;
        protected IUserChannel _userChannel;
        protected UserEthalonChannel _userEchalonChannel;
        protected IPropertyPool _propertyPool;
        private ITransportChannelType _connection;
        private double _realValue;
        private bool _accept;
        protected Action _currentAction;
        protected Dispatcher _dispatcher;
        protected IDeviceManager _deviceManager;
        private string _ethalonTypeKey;
        protected TestResult _resultPool;
        private object _ethalonChannel;
        private ITransportChannelType _ethalonChannelType;
        private bool _stopEnabled = false;

        protected CheckStateViewModel _stateViewModel;
        private string _title;
        private bool _pauseEnabled = false;
        private bool _isPaused;

        #endregion

        protected CheckBaseViewModel(CheckBase method, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool, ADTSParameters customConfig)
        {
            Method = method;
            _propertyPool = propertyPool;
            // Базовая инициализация
            //var adts = _propertyPool.ByKey(method.ChannelKey);
            Method.Init(customConfig);
            AttachEvent(method);

            _connection = Method.ChannelType;
            _userChannel = new UserChannel();
            Method.SetUserChannel(_userChannel);
            _deviceManager = deviceManager;
            _resultPool = resultPool;
            _userChannel.QueryStarted += OnQueryStarted;
            _currentAction = DoStart;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _userEchalonChannel = new UserEthalonChannel(_userChannel, TimeSpan.FromMilliseconds(100));
            //if (Method.EthalonChannelType == null)
            //{
            //    Method.SetEthalonChannel(_userEchalonChannel, null);
            //}

            Title = "ADTS";
            _stateViewModel = new CheckStateViewModel();
            _stateViewModel.TitleSteps = "Щаги";
            _stateViewModel.TitleBtnNext = "Старт";
            _stateViewModel.ADTS = new ADTSViewModel(_deviceManager) { IsControlMode = false };//new ADTSViewModel(Method.GetADTS());
            _stateViewModel.Steps = Method.Steps.Select(el => new StepViewModel(el.Step, el.Enabled));
            _stateViewModel.ResultsLog = new ObservableCollection<EventArgTestStepResult>();
        }

        #region IMethodViewModel

        public void SetAggregator(IEventAggregator agregator)
        {
            _agregator = agregator;
            if (Method != null)
                Method.SetAggregator(agregator);
        }

        /// <summary>
        /// Задать подключение для ADTS
        /// </summary>
        /// <param name="connection"></param>
        public void SetConnection(ITransportChannelType connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Установить эталонный канал
        /// </summary>
        /// <param name="ethalonTypeKey"></param>
        /// <param name="settings"></param>
        public void SetEthalonChannel(string ethalonTypeKey, ITransportChannelType settings)
        {
            if (string.IsNullOrEmpty(ethalonTypeKey) || ethalonTypeKey == UserEthalonChannel.Key || settings == null)
            {
                Method.SetEthalonChannel(_userEchalonChannel, null);
                _ethalonTypeKey = UserEthalonChannel.Key;
                _ethalonChannelType = null;
                State.IsUserChannel = true;
                EthalonChannel = _userEchalonChannel;
                return;
            }
            _ethalonTypeKey = ethalonTypeKey;
            _ethalonChannelType = settings;
            State.IsUserChannel = _ethalonTypeKey == null || _ethalonTypeKey == UserEthalonChannel.Key;
            EthalonChannel = _deviceManager.GetEthalonChannel(_ethalonTypeKey);
        }

        /// <summary>
        /// Текущий результат
        /// </summary>
        public TestResult CurrentResult{get { return _resultPool; }}

        /// <summary>
        /// Проверка запущена
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Проверка остановлена
        /// </summary>
        public event EventHandler Stoped;

        #endregion

        #region Interface of state
        /// <summary>
        /// Заголовок проверки
        /// </summary>
        public string Title
        {
            get { return _title; }
            protected set { _title = value; }
        }

        /// <summary>
        /// Модель состояния проверки
        /// </summary>
        public CheckStateViewModel State{get { return _stateViewModel; }}
        #endregion

        #region Interface of rule
        /// <summary>
        /// Установить текущую точку как целевую точку проверик
        /// </summary>
        public ICommand SetCurrentValueAsPoint { get { return new CommandWrapper(DoStopOnCurrentValue); } }

        /// <summary>
        /// Корректировать эталонное значение
        /// </summary>
        public ICommand CorrectRealValue { get { return new CommandWrapper(DoCorrectRealVal); } }

        /// <summary>
        /// Запустить проверку
        /// </summary>
        public ICommand Start { get { return new GalaSoft.MvvmLight.Command.RelayCommand(() => _currentAction()); } }

        /// <summary>
        /// Остановит проверку
        /// </summary>
        public ICommand Stop { get { return new GalaSoft.MvvmLight.Command.RelayCommand(DoCancel); } }

        /// <summary>
        /// Подтверждение установки точки
        /// </summary>
        public ICommand Accept { get { return new RelayCommand(DoAccept); } }

        /// <summary>
        /// Остановит проверку
        /// </summary>
        public ICommand PauseResume { get { return new GalaSoft.MvvmLight.Command.RelayCommand(DoPauseResume); } }

        /// <summary>
        /// Эталонное значение
        /// </summary>
        public double RealValue
        {
            get { return _realValue; }
            set { Set(ref _realValue, value); }
        }

        /// <summary>
        /// Доступна кнопка "Подтверждаю"
        /// </summary>
        public bool AcceptEnabled
        {
            get { return _accept; }
            set { Set(ref _accept, value); }
        }

        /// <summary>
        /// Доступна кнопка "Стоп"
        /// </summary>
        public bool StopEnabled
        {
            get { return _stopEnabled; }
            set { Set(ref _stopEnabled, value); }
        }

        /// <summary>
        /// Доступна кнопка пауза
        /// </summary>
        public bool PauseEnabled
        {
            get { return _pauseEnabled; }
            set { Set(ref _pauseEnabled, value); }
        }

        /// <summary>
        /// Проверка поставлена на паузу
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
            set { Set(ref _isPaused, value); }
        }

        #endregion

        #region Services
        protected virtual CheckBase Method { get; set; }

        /// <summary>
        /// Эталонный канал
        /// </summary>
        private object EthalonChannel
        {
            get { return _ethalonChannel; }
            set
            {
                Set(ref _ethalonChannel, value);
                if (_ethalonTypeKey == UserEthalonChannel.Key)
                    State.EthalonChannelViewModel = null;
                else
                    State.EthalonChannelViewModel = _deviceManager.GetEthalonChannelViewModel(_ethalonTypeKey, _ethalonChannel as IEthalonChannel);
            }
        }

        #region Event reaction
        /// <summary>
        /// Устаноить/снять с паузы
        /// </summary>
        private void DoPauseResume()
        {
            if (IsPaused)
            {
                Method.Resume();
            }
            else
            {
                Method.Pause();
            }
        }

        /// <summary>
        /// Установить текущее значение точкой проверки
        /// </summary>
        private void DoStopOnCurrentValue()
        {
            Method.SetCurrentValueAsPoint();
        }

        /// <summary>
        /// Коррекция эталонного параметра
        /// </summary>
        /// <param name="param"></param>
        private void DoCorrectRealVal(object param)
        {
            double correction;
            if (double.TryParse((string)param, NumberStyles.Any, CultureInfo.InvariantCulture, out correction))
                RealValue = RealValue + correction;
        }

        #region Обработчики кнопок последовательности
        /// <summary>
        /// Запуск проверки
        /// </summary>
        protected void DoStart()
        {
            State.TitleBtnNext = "Далее";
            Method.ChannelType = _connection;
            try
            {
                State.ADTS.Start(_connection);
            }
            catch(Exception ex) //todo поймать ошибку подключения
            {
                Debug.WriteLine(ex.ToString());
                if (_agregator!=null)
                    _agregator.Post(new ErrorMessageEventArg("Не удалось подключить АДТС"));
                // В базовое состояние
                ToStart(true);
                return;
            }
            // Задаем эталон

            if (_ethalonTypeKey != null && _ethalonChannelType != null)
            {
                try
                {
                    Method.SetEthalonChannel(_deviceManager.GetEthalonChannel(_ethalonTypeKey), _ethalonChannelType);
                }
                catch (Exception ex) //todo поймать ошибку подключения
                {
                    Debug.WriteLine(ex.ToString());
                    if (_agregator != null)
                        _agregator.Post(new ErrorMessageEventArg("Не удалось подключить эталонный канал"));
                    // В базовое состояние
                    ToStart();
                    return;
                }
            }
            else
                Method.SetEthalonChannel(_userEchalonChannel, null);
            // Запускаем
            Task.Run(() =>
            {
                // Выполнить шаг
                Method.Start();
                // В базовое состояние
                ToStart();
                // Следующее действие - следующий шаг
                _currentAction = DoStart;
            });
            // Разблокировать стоп
            StopEnabled = true;
            // Заблокировать старт
            State.WaitUserReaction = false; 
            OnStarted();
        }

        /// <summary>
        /// Переход к следующей точке
        /// </summary>
        private void DoNext()
        {
            State.TitleBtnNext = "Далее";
            State.Note = "Подождите";
            State.WaitUserReaction = false;
            if (_userChannel.QueryType == UserQueryType.GetRealValue)
            {
                _userChannel.RealValue = RealValue;
                _userChannel.AgreeValue = true;
            }
        }

        /// <summary>
        /// Подтверждение ввода эталонного здачения
        /// </summary>
        private void DoAccept()
        {
            State.TitleBtnNext = "Старт";
            if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                _userChannel.AcceptValue = true;
                _userChannel.AgreeValue = true;
            }
            AcceptEnabled = false;
            State.WaitUserReaction = false;
        }

        /// <summary>
        /// Отмена проверки
        /// </summary>
        private void DoCancel()
        {
            State.TitleBtnNext = "Старт";

            Method.Stop();

            if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                _userChannel.AcceptValue = false;
                _userChannel.AgreeValue = true;
            }
            State.WaitUserReaction = true;
            AcceptEnabled = false;
            StopEnabled = false;
        }

        /// <summary>
        /// В исходное состояние
        /// </summary>
        /// <param name="withoutDevices">Выполнить останов без устройств</param>
        private void ToStart(bool withoutDevices = false)
        {
            State.TitleBtnNext = "Старт";

            if (!withoutDevices)
                Method.Stop();

            if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                _userChannel.AcceptValue = false;
                _userChannel.AgreeValue = true;
            }
            State.WaitUserReaction = true;
            AcceptEnabled = false;
            StopEnabled = false;
        }
        #endregion

        #region Обработчики событий
        /// <summary>
        /// Проверка запущена
        /// </summary>
        protected virtual void OnStarted()
        {
            EventHandler handler = Started;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Провека остановлена
        /// </summary>
        protected virtual void OnStoped()
        {
            EventHandler handler = Stoped;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Изменился набор шагов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected void OnStepsChanged(object sender, EventArgs eventArgs)
        {
            State.Steps = new ObservableCollection<StepViewModel>(Method.Steps.Select(el => new StepViewModel(el.Step, el.Enabled)));
        }

        /// <summary>
        /// Значение эталона затребовано по каналу пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnQueryStarted(object sender, EventArgs e)
        {
            if (_userChannel.QueryType == UserQueryType.GetRealValue)
            {
                State.TitleBtnNext = "Далее";
                State.WaitUserReaction = true;
                State.Note = string.Format("Укажите эталонное значение и нажмите \"{0}\"", State.TitleBtnNext);
                RealValue = _userChannel.RealValue;
                _currentAction = DoNext;
            }
            else if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                State.TitleBtnNext = "Отмена";
                State.WaitUserReaction = true;
                State.Note = string.Format(_userChannel.Message, "Подтвердить");//string.Format("Что бы применить результат калибровки нажмите \"Подтвердить\", в противном случае нажмите \"{0}\"", State.TitleBtnNext);
                AcceptEnabled = true;
                _currentAction = DoCancel;
            }
        }

        /// <summary>
        /// Получен результат от очередного шага проверки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgTestResult"></param>
        protected void ResultUpdated(object sender, EventArgTestStepResult eventArgTestResult)
        {
            _dispatcher.Invoke(() =>
            {
                var newRes = eventArgTestResult.Result as AdtsPointResult;
                State.ResultsLog.Add(eventArgTestResult);
                AdtsPointResult fidedRes = null;
                if (newRes == null)
                {
                    // новый результат не AdtsPointResult, потому не с чем сравнивать
                    _resultPool.Results.Add(new TestStepResult(Method.Key, Method.ChannelKey, eventArgTestResult.Key, eventArgTestResult.Result));
                    return;
                }

                // поиск точки в имеющихся результатах
                foreach (var stepResult in _resultPool.Results)
                {
                    var res = stepResult.Result as AdtsPointResult;
                    if (res == null)
                        continue;
                    if (Math.Abs(res.Point - newRes.Point) < double.Epsilon &&
                        stepResult.CheckKey == Method.Key &&
                        stepResult.ChannelKey == Method.ChannelKey)
                    {
                        fidedRes = res;
                        break;
                    }
                }

                if (fidedRes == null)
                {
                    // это новая точка
                    _resultPool.Results.Add(new TestStepResult(Method.Key, Method.ChannelKey, eventArgTestResult.Key, eventArgTestResult.Result));
                    return;
                }

                fidedRes.RealValue = newRes.RealValue;
                fidedRes.Tolerance = newRes.Tolerance;
                fidedRes.Error = newRes.Error;
                fidedRes.IsCorrect = newRes.IsCorrect;
            });
        }

        void EndMethod(object sender, EventArgs e)
        {
            OnStoped();
        }

        void model_PauseAvailableChanged(object sender, EventArgs e)
        {
            PauseEnabled = Method.IsPauseAvailable;
        }
        #endregion

        #endregion

        private void AttachEvent(CheckBase model)
        {
            model.StepsChanged += OnStepsChanged;
            model.ResultUpdated += ResultUpdated;
            model.EndMethod += EndMethod;
            model.PauseAvailableChanged += model_PauseAvailableChanged;
        }

        private void DetachEvent(CheckBase model)
        {
            model.StepsChanged -= OnStepsChanged;
            model.ResultUpdated -= ResultUpdated;
            model.EndMethod -= EndMethod;
            model.PauseAvailableChanged -= model_PauseAvailableChanged;
        }

        public override void Cleanup()
        {
            if (Method != null)
                DetachEvent(Method);
            if (_userChannel != null)
                _userChannel.QueryStarted -= OnQueryStarted;
            base.Cleanup();
        }
        #endregion
    }
}