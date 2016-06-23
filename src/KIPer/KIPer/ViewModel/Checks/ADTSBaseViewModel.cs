using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Archive.DTO;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel.Channels;
using KipTM.ViewModel.Services;

namespace KipTM.ViewModel.Checks
{
    public abstract class ADTSBaseViewModel : ViewModelBase, IMethodViewModel
    {
        private string _titleBtnNext;
        protected IUserChannel _userChannel;
        protected UserEthalonChannel _userEchalonChannel;
        private IEnumerable<StepViewModel> _steps;
        protected IPropertyPool _propertyPool;
        private bool _waitUserReaction;
        private ITransportChannelType _connection;
        private double _realValue;
        private bool _accept;
        private string _note;
        protected Action _currentAction;
        protected Dispatcher _dispatcher;
        protected IDeviceManager _deviceManager;
        private string _ethalonTypeKey;
        private ITransportChannelType _ethalonChannelType;
        protected TestResult _resultPool;
        protected ADTSViewModel _adtsViewModel;
        private bool _isUserChannel;
        private object _ethalonChannel;
        private object _ethalonChannelViewModel;
        private bool _stopEnabled = false;

        protected ADTSBaseViewModel(ADTSMethodBase method, IPropertyPool propertyPool,
            IDeviceManager deviceManager, TestResult resultPool)
        {
            Method = method;
            _propertyPool = propertyPool;
            // ������� �������������
            var adts = _propertyPool.ByKey(method.ChannelKey);
            Method.Init(adts);
            AttachEvent(method);

            Steps = Method.Steps.Select(el => new StepViewModel(el));
            
            _userChannel = new UserChannel();
            _userEchalonChannel = new UserEthalonChannel(_userChannel, TimeSpan.FromMilliseconds(100));
            _deviceManager = deviceManager;
            _resultPool = resultPool;
            _adtsViewModel = new ADTSViewModel(_deviceManager);
            Results = new ObservableCollection<EventArgTestStepResult>();
            _userChannel.QueryStarted += OnQueryStarted;
            _currentAction = DoStart;
            _dispatcher = Dispatcher.CurrentDispatcher;
            TitleBtnNext = "�����";
        }


        #region Interface for config
        /// <summary>
        /// �������� ��������
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// �������� �����������
        /// </summary>
        public event EventHandler Stoped;

        #endregion

        #region Interface of state
        /// <summary>
        /// ������������� ���������� ADTS
        /// </summary>
        public ADTSViewModel ADTS
        {
            get { return _adtsViewModel; }
        }

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

        /// <summary>
        /// ��������� ����� ��������
        /// </summary>
        public IEnumerable<StepViewModel> Steps
        {
            get { return _steps; }
            set { Set(ref _steps, value); }
        }

        /// <summary>
        /// ������������� ���������� ������
        /// </summary>
        public object EthalonChannelViewModel
        {
            get { return _ethalonChannelViewModel; }
            set { Set(ref _ethalonChannelViewModel, value); }
        }

        /// <summary>
        /// �������� ������ �����/�����
        /// </summary>
        public string TitleBtnNext
        {
            get { return _titleBtnNext; }
            set { Set(ref _titleBtnNext, value); }
        }

        /// <summary>
        /// ���������� � �������
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { Set(ref _note, value); }
        }

        /// <summary>
        /// ������� ������� ������������
        /// </summary>
        public bool WaitUserReaction
        {
            get { return _waitUserReaction; }
            set { Set(ref _waitUserReaction, value); }
        }

        /// <summary>
        /// ����������
        /// </summary>
        public ObservableCollection<EventArgTestStepResult> Results { get; private set; }

        #endregion

        #region Interface of rule
        /// <summary>
        /// ���������� ������� ����� ��� ������� ����� ��������
        /// </summary>
        public ICommand SetCurrentValueAsPoint { get { return new CommandWrapper(DoStopOnCurrentValue); } }

        /// <summary>
        /// �������������� ��������� ��������
        /// </summary>
        public ICommand CorrectRealValue { get { return new CommandWrapper(DoCorrectRealVal); } }

        /// <summary>
        /// ��������� ��������
        /// </summary>
        public ICommand Start { get { return new GalaSoft.MvvmLight.Command.RelayCommand(() => _currentAction()); } }

        /// <summary>
        /// ��������� ��������
        /// </summary>
        public ICommand Stop { get { return new GalaSoft.MvvmLight.Command.RelayCommand(DoCancel); } }

        /// <summary>
        /// ������������� ��������� �����
        /// </summary>
        public ICommand Accept { get { return new RelayCommand(DoAccept); } }

        /// <summary>
        /// ��������� ��������
        /// </summary>
        public double RealValue
        {
            get { return _realValue; }
            set { Set(ref _realValue, value); }
        }

        /// <summary>
        /// �������� ������ "�����������"
        /// </summary>
        public bool AcceptEnabled
        {
            get { return _accept; }
            set { Set(ref _accept, value); }
        }

        /// <summary>
        /// �������� ������ "����"
        /// </summary>
        public bool StopEnabled
        {
            get { return _stopEnabled; }
            set { Set(ref _stopEnabled, value); }
        }

        #endregion

        #region Interface of config
        /// <summary>
        /// ������ ����������� ��� ADTS
        /// </summary>
        /// <param name="connection"></param>
        public void SetConnection(ITransportChannelType connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// ���������� ��������� �����
        /// </summary>
        /// <param name="ethalonTypeKey"></param>
        /// <param name="settings"></param>
        public void SetEthalonChannel(string ethalonTypeKey, ITransportChannelType settings)
        {
            _ethalonTypeKey = ethalonTypeKey;
            _ethalonChannelType = settings;
            IsUserChannel = _ethalonTypeKey == null;
            EthalonChannel = _deviceManager.GetEthalonChannel(_ethalonTypeKey, _ethalonChannelType);
        }

        /// <summary>
        /// ���������� ������������ ��������� �������
        /// </summary>
        public void SlectUserEthalonChannel()
        {
            Method.SetEthalonChannel(_userEchalonChannel, null);
            _ethalonTypeKey = null;
            _ethalonChannelType = null;
            IsUserChannel = true;
            EthalonChannel = null;
        }

        #endregion

        #region Services
        protected virtual ADTSMethodBase Method { get; set; }

        /// <summary>
        /// ��������� �����
        /// </summary>
        private object EthalonChannel
        {
            get { return _ethalonChannel; }
            set
            {
                Set(ref _ethalonChannel, value);
                EthalonChannelViewModel = GetViewModelForChannel(_ethalonChannel);
            }
        }

        #region Event reaction
        /// <summary>
        /// ���������� ������� �������� ������ ��������
        /// </summary>
        private void DoStopOnCurrentValue()
        {
            Method.SetCurrentValueAsPoint();
        }

        /// <summary>
        /// ��������� ���������� ���������
        /// </summary>
        /// <param name="param"></param>
        private void DoCorrectRealVal(object param)
        {
            double correction;
            if (double.TryParse((string)param, NumberStyles.Any, CultureInfo.InvariantCulture, out correction))
                RealValue = RealValue + correction;
        }

        /// <summary>
        /// ������ ��������
        /// </summary>
        protected void DoStart()
        {
            TitleBtnNext = "�����";
            //var visaSett = _connection.SelectedChannel.Settings as VisaSettings;
            //if (visaSett != null)
            //    visaSett.Address = _connection.Address;
            Method.ChannelType = _connection;
            _adtsViewModel.Start(Method.GetADTS(), _connection);
            // ������ ������
            if (_ethalonTypeKey != null && _ethalonChannelType != null)
                Method.SetEthalonChannel(_deviceManager.GetEthalonChannel(_ethalonTypeKey, _ethalonChannelType), _ethalonChannelType);
            else
                Method.SetEthalonChannel(_userEchalonChannel, null);
            // ���������
            Task.Run(() =>
            {
                Method.Start();
                DoCancel();
                _currentAction = DoStart;
            });
            StopEnabled = true;
            OnStarted();
        }

        /// <summary>
        /// ������� � ��������� �����
        /// </summary>
        private void DoNext()
        {
            TitleBtnNext = "�����";
            if (_userChannel.QueryType == UserQueryType.GetRealValue)
            {
                _userChannel.RealValue = RealValue;
                _userChannel.AgreeValue = true;
            }
        }

        /// <summary>
        /// ������������� ����� ���������� ��������
        /// </summary>
        private void DoAccept()
        {
            TitleBtnNext = "�����";
            if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                _userChannel.AcceptValue = true;
                _userChannel.AgreeValue = true;
            }
            AcceptEnabled = false;
            OnStoped();
        }

        /// <summary>
        /// ������ ��������
        /// </summary>
        private void DoCancel()
        {
            TitleBtnNext = "�����";

            Method.Stop();

            if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                _userChannel.AcceptValue = false;
                _userChannel.AgreeValue = true;
            }
            AcceptEnabled = false;
            StopEnabled = false;
            OnStoped();
        }

        /// <summary>
        /// �������� ��������
        /// </summary>
        protected virtual void OnStarted()
        {
            EventHandler handler = Started;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// ������� �����������
        /// </summary>
        protected virtual void OnStoped()
        {
            EventHandler handler = Stoped;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// ��������� ����� �����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        protected void OnStepsChanged(object sender, EventArgs eventArgs)
        {
            Steps = new ObservableCollection<StepViewModel>(Method.Steps.Select(el => new StepViewModel(el)));
        }

        /// <summary>
        /// �������� ������� ����������� �� ������ ������������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnQueryStarted(object sender, EventArgs e)
        {
            if (_userChannel.QueryType == UserQueryType.GetRealValue)
            {
                TitleBtnNext = "�����";
                Note = string.Format("������� ��������� �������� � ������� \"{0}\"", TitleBtnNext);
                RealValue = _userChannel.RealValue;
                _currentAction = DoNext;
            }
            else if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                TitleBtnNext = "������";
                Note = string.Format("��� �� ��������� ��������� ���������� ������� \"�����������\", � ��������� ������ ������� \"{0}\"", TitleBtnNext);
                AcceptEnabled = true;
                _currentAction = DoCancel;
            }
        }

        /// <summary>
        /// ������� ��������� �� ���������� ���� ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgTestResult"></param>
        protected void ResultUpdated(object sender, EventArgTestStepResult eventArgTestResult)
        {
            _dispatcher.Invoke(() =>
            {
                Results.Add(eventArgTestResult);
                _resultPool.Results.Add(eventArgTestResult.Key, eventArgTestResult.Result);
            });
        }

        void EndMethod(object sender, EventArgs e)
        {
            _resultPool.Results = M ;
        }
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

        object GetViewModelForChannel(object model) //TODO ���������� ��������� ���������� ������ �� �������� ������
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