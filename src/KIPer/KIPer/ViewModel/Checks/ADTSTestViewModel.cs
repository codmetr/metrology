using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using ADTS;
using ArchiveData.DTO.Params;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Archive.DTO;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using KipTM.Settings;
using KipTM.ViewModel;
using KipTM.ViewModel.Channels;
using KipTM.ViewModel.Services;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    [MethodicViewModelAttribute(typeof(ADTSTestMethod))]
    public class ADTSTestViewModel : ViewModelBase, IMethodViewModel
    {
        private string _titleBtnNext;
        private ADTSTestMethod _methodic;
        private IUserChannel _userChannel;
        private UserEthalonChannel _userEchalonChannel;
        private IPropertyPool _propertyPool;
        private bool _waitUserReaction;

        private ITransportChannelType _connection;

        private double _realValue;

        private CancellationTokenSource _cancellation;
        private bool _accept;
        private IEnumerable<StepViewModel> _steps;
        private string _note;
        private Action _currentAction;
        private Dispatcher _dispatcher;

        private IDeviceManager _deviceManager;
        private string _ethalonTypeKey;
        private ITransportChannelType _ethalonChannelType;
        private TestResult _resultPool;
        private ADTSViewModel _adtsViewModel;
        private bool _isUserChannel;
        private object _ethalonChannel;
        private object _ethalonChannelViewModel;
        private bool _stopEnabled = false;

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSTestViewModel(ADTSTestMethod methodic, IPropertyPool propertyPool, IDeviceManager deviceManager, TestResult resultPool)
        {
            _cancellation = new CancellationTokenSource();
            _userChannel = new UserChannel();
            _userEchalonChannel = new UserEthalonChannel(_userChannel, TimeSpan.FromMilliseconds(100));
            _methodic = methodic;
            _propertyPool = propertyPool;
            _deviceManager = deviceManager;
            _resultPool = resultPool;
            _adtsViewModel = new ADTSViewModel(_deviceManager);

            // Базовая инициализация
            var adts = _propertyPool.ByKey(methodic.ChannelKey);
            _methodic.Init(adts);
            _methodic.StepsChanged += OnStepsChanged;
            _methodic.ResultUpdated += ResultUpdated;

            _userChannel.QueryStarted += _userChannel_QueryStarted;

            Results = new ObservableCollection<KeyValuePair<ParameterDescriptor, ParameterResult>>();
            Steps = _methodic.Steps.Select(el=>new StepViewModel(el));
            TitleBtnNext = "Старт";
            _currentAction = DoStart;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        #region Interface for config
        public event EventHandler Started;

        public event EventHandler Stoped;

        public ADTSViewModel ADTS
        {
            get { return _adtsViewModel; }
        }

        public void SetConnection(ITransportChannelType connection)
        {
            _connection = connection;
        }

        public void SetEthalonChannel(string ethalonTypeKey, ITransportChannelType settings)
        {
            _ethalonTypeKey = ethalonTypeKey;
            _ethalonChannelType = settings;
            IsUserChannel = _ethalonTypeKey == null;
            EthalonChannel = _deviceManager.GetEthalonChannel(_ethalonTypeKey, _ethalonChannelType);
        }

        public void SlectUserEthalonChannel()
        {
            _methodic.SetEthalonChannel(_userEchalonChannel, null);
            _ethalonTypeKey = null;
            _ethalonChannelType = null;
            IsUserChannel = true;
            EthalonChannel = null;
        }
        #endregion

        #region Properties for View

        public bool IsUserChannel
        {
            get { return _isUserChannel; }
            set
            {
                Set(ref _isUserChannel, value);
                RaisePropertyChanged("IsNotUserChannel");
            }
        }

        public bool IsNotUserChannel
        {
            get { return !_isUserChannel; }
        }

        public object EthalonChannel
        {
            get { return _ethalonChannel; }
            set
            {
                Set(ref _ethalonChannel, value);
                EthalonChannelViewModel = GetViewModelForChannel(_ethalonChannel);
            }
        }

        public object EthalonChannelViewModel
        {
            get { return _ethalonChannelViewModel; }
            set { Set(ref _ethalonChannelViewModel, value); }
        }

        public string TitleBtnNext
        {
            get { return _titleBtnNext; }
            set { Set(ref _titleBtnNext, value); }
        }

        public string Note
        {
            get { return _note; }
            set { Set(ref _note, value); }
        }

        public bool WaitUserReaction
        {
            get { return _waitUserReaction; }
            set { Set(ref _waitUserReaction, value); }
        }

        public ObservableCollection<KeyValuePair<ParameterDescriptor, ParameterResult>> Results { get; private set; }

        public ICommand SetCurrentValueAsPoint { get { return new CommandWrapper(DoStopOnCurrentValue); } }
        
        public ICommand CorrectRealValue { get { return new CommandWrapper(DoCorrectRealVal); } }

        public ICommand Start { get { return new GalaSoft.MvvmLight.Command.RelayCommand(()=>_currentAction()); } }

        public ICommand Stop { get { return new GalaSoft.MvvmLight.Command.RelayCommand(DoCancel); } }

        public ICommand Accept { get { return new RelayCommand(DoAccept); } }

        public IEnumerable<StepViewModel> Steps
        {
            get { return _steps; }
            set { Set(ref _steps, value); }
        }

        public double RealValue
        {
            get { return _realValue; }
            set { Set(ref _realValue, value); }
        }

        public bool AcceptEnabled
        {
            get { return _accept; }
            set { Set(ref _accept, value); }
        }

        public bool StopEnabled
        {
            get { return _stopEnabled; }
            set { Set(ref _stopEnabled, value); }
        }
        #endregion

        #region Service methods
        private void ResultUpdated(object sender, EventArgTestResult eventArgTestResult)
        {
            foreach (var result in eventArgTestResult.Result)
            {
                _dispatcher.Invoke(() =>
                {
                    Results.Add(result);
                });
                _resultPool.Results.Add(result.Key, result.Value);
            }
        }

        private void DoStopOnCurrentValue()
        {
            _methodic.SetCurrentValueAsPoint();
        }

        private void DoCorrectRealVal(object param)
        {
            double correction;
            if (double.TryParse((string) param, NumberStyles.Any, CultureInfo.InvariantCulture, out correction))
                RealValue = RealValue + correction;
        }

        private void DoStart()
        {
            TitleBtnNext = "Далее";
            //var visaSett = _connection.SelectedChannel.Settings as VisaSettings;
            //if (visaSett != null)
            //    visaSett.Address = _connection.Address;
            _methodic.ChannelType = _connection;
            _adtsViewModel.Start(_methodic.GetADTS(),_connection);
            // Задаем эталон
            if (_ethalonTypeKey != null && _ethalonChannelType != null)
                _methodic.SetEthalonChannel(_deviceManager.GetEthalonChannel(_ethalonTypeKey, _ethalonChannelType), _ethalonChannelType);
            else
                _methodic.SetEthalonChannel(_userEchalonChannel, null);
            // Запускаем
            Task.Run(() =>
            {
                _methodic.Start();
                DoCancel();
                _currentAction = DoStart;
            });
            StopEnabled = true;
            OnStarted();
        }

        private void DoNext()
        {
            TitleBtnNext = "Далее";
            if (_userChannel.QueryType == UserQueryType.GetRealValue)
            {
                _userChannel.RealValue = RealValue;
                _userChannel.AgreeValue = true;
            }
        }

        private void DoAccept()
        {
            TitleBtnNext = "Старт";
            if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                _userChannel.AcceptValue = true;
                _userChannel.AgreeValue = true;
            }
            AcceptEnabled = false;
            OnStoped();
        }

        private void DoCancel()
        {
            TitleBtnNext = "Старт";

            _methodic.Stop();

            if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                _userChannel.AcceptValue = false;
                _userChannel.AgreeValue = true;
            }
            AcceptEnabled = false;
            StopEnabled = false;
            OnStoped();
        }

        protected virtual void OnStarted()
        {
            EventHandler handler = Started;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnStoped()
        {
            EventHandler handler = Stoped;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnStepsChanged(object sender, EventArgs eventArgs)
        {
            Steps = new ObservableCollection<StepViewModel>(_methodic.Steps.Select(el => new StepViewModel(el)));
        }

        private void _userChannel_QueryStarted(object sender, EventArgs e)
        {
            if (_userChannel.QueryType == UserQueryType.GetRealValue)
            {
                TitleBtnNext = "Далее";
                Note = string.Format("Укажите эталонное значение и нажмите \"{0}\"", TitleBtnNext);
                RealValue = _userChannel.RealValue;
                _currentAction = DoNext;
            }
            else if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                TitleBtnNext = "Отмена";
                Note = string.Format("Что бы применить результат калибровки нажмите \"Подтвердить\", в противном случае нажмите \"{0}\"", TitleBtnNext);
                AcceptEnabled = true;
                _currentAction = DoCancel;
            }
        }

        object GetViewModelForChannel(object model)
        {
            if (model is PACEEthalonChannel)
            {
                return new PACEEchalonChannelViewModel(model as PACEEthalonChannel);
            }
            return null;
        }

        #endregion

        public override void Cleanup()
        {
            if (_methodic != null && _methodic.StepsChanged != null) _methodic.StepsChanged -= OnStepsChanged;
            if (_userChannel != null) _userChannel.QueryStarted -= _userChannel_QueryStarted;
            base.Cleanup();
        }
    }
}