﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ADTS;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Settings;
using KipTM.ViewModel;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    [MethodicViewModelAttribute(typeof(ADTSCheckMethod))]
    public class ADTSCalibrationViewModel : ViewModelBase, IMethodViewModel
    {
        private string _titleBtnNext;
        private ADTSCheckMethod _methodic;
        private IUserChannel _userChannel;
        private UserEchalonChannel _userEchalonChannel;
        private IPropertyPool _propertyPool;
        private bool _waitUserReaction;

        private double _realValue;

        private CancellationTokenSource _cancellation;
        private bool _accept;
        private IEnumerable<StepViewModel> _steps;
        private string _note;
        private Action _currentAction;

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCalibrationViewModel(ADTSCheckMethod methodic, IPropertyPool propertyPool)
        {
            _cancellation = new CancellationTokenSource();
            _userChannel = new UserChannel();
            _userEchalonChannel = new UserEchalonChannel(_userChannel, TimeSpan.FromMilliseconds(100));
            _methodic = methodic;
            _propertyPool = propertyPool;
            
            // Базовая инициализация
            var adts = _propertyPool.ByKey(methodic.ChannelKey);
            _methodic.Init(adts);
            _methodic.StepsChanged += OnStepsChanged;

            _userChannel.QueryStarted += _userChannel_QueryStarted;

            Results = new ObservableCollection<ParameterResultViewModel>();
            Steps = _methodic.Steps.Select(el=>new StepViewModel(el));
            TitleBtnNext = "Старт";
            _currentAction = DoStart;
        }

        public void SlectUserEthalonChannel()
        {
            _methodic.SetEthalonChannel(_userEchalonChannel);
        }

        public void SetEthalonChannel(IEthalonChannel ethalon)
        {
            _methodic.SetEthalonChannel(ethalon);
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

        public ObservableCollection<ParameterResultViewModel> Results { get; private set; }

        public ICommand CorrectRealValue { get { return new CommandWrapper(DoCorrectRealVal); } }

        public ICommand Start { get { return new GalaSoft.MvvmLight.Command.RelayCommand(_currentAction); } }

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

        private void DoCorrectRealVal(object param)
        {
            double correction;
            if (double.TryParse((string) param, NumberStyles.Any, CultureInfo.InvariantCulture, out correction))
                RealValue = RealValue + correction;
        }

        private void DoStart()
        {
            TitleBtnNext = "Далее";
            Task.Run(()=>_methodic.Start());
        }

        private void DoNext()
        {
            TitleBtnNext = "Далее";
            if (_userChannel.QueryType == UserQueryType.GetAccept)
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
        }

        private void DoCancel()
        {
            TitleBtnNext = "Старт";
            if (_userChannel.QueryType == UserQueryType.GetAccept)
            {
                _userChannel.AcceptValue = false;
                _userChannel.AgreeValue = true;
            }
            AcceptEnabled = false;
        }

        private void OnStepsChanged(object sender, EventArgs eventArgs)
        {
            Steps = new ObservableCollection<StepViewModel>(_methodic.Steps.Select(el => new StepViewModel(el)));
        }

        void _userChannel_QueryStarted(object sender, EventArgs e)
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

        public override void Cleanup()
        {
            if (_methodic != null && _methodic.StepsChanged != null) _methodic.StepsChanged -= OnStepsChanged;
            if (_userChannel != null) _userChannel.QueryStarted -= _userChannel_QueryStarted;
            base.Cleanup();
        }
    }
}