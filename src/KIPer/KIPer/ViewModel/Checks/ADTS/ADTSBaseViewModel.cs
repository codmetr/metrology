using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using ArchiveData.DTO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel.Channels;
using KipTM.ViewModel.Master;
using KipTM.ViewModel.Services;

namespace KipTM.ViewModel.Checks
{
    public abstract class ADTSBaseViewModel : ViewModelBase, IMethodViewModel
    {
        #region Members
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

        protected AdtsCheckStateViewModel _stateViewModel;
        private string _title;

        #endregion

        protected ADTSBaseViewModel(ADTSMethodBase method, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool, ADTSMethodParameters customConfig)
        {
            Method = method;
            _propertyPool = propertyPool;
            // Базовая инициализация
            var adts = _propertyPool.ByKey(method.ChannelKey);
            Method.Init(customConfig);
            AttachEvent(method);

            _connection = Method.ChannelType;
            _userChannel = new UserChannel();
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
            _stateViewModel = new AdtsCheckStateViewModel();
            _stateViewModel.TitleSteps = "Щаги";
            _stateViewModel.TitleBtnNext = "Старт";
            _stateViewModel.ADTS = new ADTSViewModel(_deviceManager);
            _stateViewModel.Steps = Method.Steps.Select(el => new StepViewModel(el.Step, el.Enabled));
            _stateViewModel.ResultsLog = new ObservableCollection<EventArgTestStepResult>();
        }

        #region IMethodViewModel

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
                _ethalonTypeKey = null;
                _ethalonChannelType = null;
                State.IsUserChannel = true;
                EthalonChannel = _userEchalonChannel;
                return;
            }
            _ethalonTypeKey = ethalonTypeKey;
            _ethalonChannelType = settings;
            State.IsUserChannel = _ethalonTypeKey == null;
            EthalonChannel = _deviceManager.GetEthalonChannel(_ethalonTypeKey, _ethalonChannelType);
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

        public string Title
        {
            get { return _title; }
            protected set { _title = value; }
        }

        public AdtsCheckStateViewModel State{get { return _stateViewModel; }}
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

        #endregion

        #region Services
        protected virtual ADTSMethodBase Method { get; set; }

        /// <summary>
        /// Эталонный канал
        /// </summary>
        private object EthalonChannel
        {
            get { return _ethalonChannel; }
            set
            {
                Set(ref _ethalonChannel, value);
                State.EthalonChannelViewModel = GetViewModelForChannel(_ethalonChannel);
            }
        }

        #region Event reaction
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
            State.ADTS.Start(Method.GetADTS(), _connection);
            // Задаем эталон
            if (_ethalonTypeKey != null && _ethalonChannelType != null)
                Method.SetEthalonChannel(_deviceManager.GetEthalonChannel(_ethalonTypeKey, _ethalonChannelType), _ethalonChannelType);
            else
                Method.SetEthalonChannel(_userEchalonChannel, null);
            // Запускаем
            Task.Run(() =>
            {
                Method.Start();
                DoCancel();
                _currentAction = DoStart;
            });
            // Разблокировать стоп
            // Заблокировать старт
            StopEnabled = true;
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
            OnStoped();
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
            OnStoped();
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
                State.Note = string.Format("Что бы применить результат калибровки нажмите \"Подтвердить\", в противном случае нажмите \"{0}\"", State.TitleBtnNext);
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
                State.ResultsLog.Add(eventArgTestResult);
                _resultPool.Results.Add(new TestStepResult(Method.Key, eventArgTestResult.Key, eventArgTestResult.Result));
            });
        }

        void EndMethod(object sender, EventArgs e)
        {
            //_resultPool.Results = M ;
        }
        #endregion

        #endregion

        private void AttachEvent(ADTSMethodBase model)
        {
            model.StepsChanged += OnStepsChanged;
            model.ResultUpdated += ResultUpdated;
            model.EndMethod += EndMethod;
        }

        private void DetachEvent(ADTSMethodBase model)
        {
            model.StepsChanged -= OnStepsChanged;
            model.ResultUpdated -= ResultUpdated;
            model.EndMethod -= EndMethod;
        }

        object GetViewModelForChannel(object model) //TODO обеспечить получение визуальной модели по реальной модели
        {
            if (model is PACEEthalonChannel)
            {
                return new PACEEchalonChannelViewModel(model as PACEEthalonChannel);
            }
            return null;
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