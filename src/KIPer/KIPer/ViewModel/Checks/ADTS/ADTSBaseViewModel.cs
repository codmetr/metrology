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
            // ������� �������������
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
            _stateViewModel.TitleSteps = "����";
            _stateViewModel.TitleBtnNext = "�����";
            _stateViewModel.ADTS = new ADTSViewModel(_deviceManager);
            _stateViewModel.Steps = Method.Steps.Select(el => new StepViewModel(el.Step, el.Enabled));
            _stateViewModel.ResultsLog = new ObservableCollection<EventArgTestStepResult>();
        }

        #region IMethodViewModel

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
        /// ������� ���������
        /// </summary>
        public TestResult CurrentResult{get { return _resultPool; }}

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

        public string Title
        {
            get { return _title; }
            protected set { _title = value; }
        }

        public AdtsCheckStateViewModel State{get { return _stateViewModel; }}
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
                State.EthalonChannelViewModel = GetViewModelForChannel(_ethalonChannel);
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

        #region ����������� ������ ������������������
        /// <summary>
        /// ������ ��������
        /// </summary>
        protected void DoStart()
        {
            
            State.TitleBtnNext = "�����";
            Method.ChannelType = _connection;
            State.ADTS.Start(Method.GetADTS(), _connection);
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
            // �������������� ����
            // ������������� �����
            StopEnabled = true;
            State.WaitUserReaction = false; 
            OnStarted();
        }

        /// <summary>
        /// ������� � ��������� �����
        /// </summary>
        private void DoNext()
        {
            State.TitleBtnNext = "�����";
            State.Note = "���������";
            State.WaitUserReaction = false;
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
            State.TitleBtnNext = "�����";
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
        /// ������ ��������
        /// </summary>
        private void DoCancel()
        {
            State.TitleBtnNext = "�����";

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

        #region ����������� �������
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
            State.Steps = new ObservableCollection<StepViewModel>(Method.Steps.Select(el => new StepViewModel(el.Step, el.Enabled)));
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
                State.TitleBtnNext = "�����";
                State.WaitUserReaction = true;
                State.Note = string.Format("������� ��������� �������� � ������� \"{0}\"", State.TitleBtnNext);
                RealValue = _userChannel.RealValue;
                _currentAction = DoNext;
            }
            else if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                State.TitleBtnNext = "������";
                State.WaitUserReaction = true;
                State.Note = string.Format("��� �� ��������� ��������� ���������� ������� \"�����������\", � ��������� ������ ������� \"{0}\"", State.TitleBtnNext);
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