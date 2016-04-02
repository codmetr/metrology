using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using ADTS;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
        private UserChannel _userChannel;
        private UserEchalonChannel _userEchalonChannel;
        private IPropertyPool _propertyPool;
        private bool _waitUserReaction;

        private double _realValue;

        private CancellationTokenSource _cancellation;
        private bool _accept;
        private IEnumerable<StepViewModel> _steps;

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

            Results = new ObservableCollection<ParameterResultViewModel>();
            Steps = _methodic.Steps.Select(el=>new StepViewModel(el));
            TitleBtnNext = "Старт";
        }

        public string TitleBtnNext
        {
            get { return _titleBtnNext; }
            set { Set(ref _titleBtnNext, value); }
        }

        public bool WaitUserReaction
        {
            get { return _waitUserReaction; }
            set { Set(ref _waitUserReaction, value); }
        }

        public ObservableCollection<ParameterResultViewModel> Results { get; private set; }

        public ICommand Start { get { return new GalaSoft.MvvmLight.Command.RelayCommand(DoStart); } }

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

        public bool Accept
        {
            get { return _accept; }
            set { Set(ref _accept, value); }
        }

        private void DoStart()
        {
            TitleBtnNext = "Далее";
            _methodic.Start();
        }

        private void OnStepsChanged(object sender, EventArgs eventArgs)
        {
            Steps = new ObservableCollection<StepViewModel>(_methodic.Steps.Select(el => new StepViewModel(el)));
        }

        public override void Cleanup()
        {
            if (_methodic.StepsChanged != null) _methodic.StepsChanged -= OnStepsChanged;
            base.Cleanup();
        }
    }
}