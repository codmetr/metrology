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
    [MethodicViewModelAttribute(typeof(ADTSCheckMethodic))]
    public class ADTSCalibrationViewModel : ViewModelBase
    {
        private string _titleBtnNext;
        private ADTSCheckMethodic _methodic;
        private UserChannel _userChannel;
        private UserEchalonChannel _userEchalonChannel;
        private IPropertyPool _propertyPool;
        private bool _waitUserReaction;

        private double _realValue;

        private CancellationTokenSource _cancellation;
        private bool _accept;

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCalibrationViewModel(ADTSCheckMethodic methodic, MainSettings settings, IPropertyPool propertyPool)
        {
            _cancellation = new CancellationTokenSource();
            _userChannel = new UserChannel();
            _userEchalonChannel = new UserEchalonChannel(_userChannel, TimeSpan.FromMilliseconds(100));
            _methodic = methodic;
            _propertyPool = propertyPool;
            // Базовая инициализация
            var points = _propertyPool.GetProperty<List<ADTSChechPoint>>(ADTSCheckMethodic.KeyPoints);
            var channel = _propertyPool.GetProperty<CalibChannel>(ADTSCheckMethodic.KeyChannel);
            _methodic.Init(new ADTSCheckParameters(channel, points));

            Points = new List<PointCheckableViewModel>(methodic.Steps.Select(el=>new PointCheckableViewModel(el.Name)));
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

        public IEnumerable<PointCheckableViewModel> Points { get; private set; }

        public ICommand NextStep { get { return new RelayCommand(DoNextStep); } }

        public ICommand Start { get { return new GalaSoft.MvvmLight.Command.RelayCommand(DoStart); } }

        public ObservableCollection<StepViewModel> Steps { get; set; }

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
        }

        private void DoNextStep()
        {}

        private void End()
        {}

        private void Cancel()
        {
            _methodic.Cancel();
            _cancellation.Cancel();
            _cancellation = new CancellationTokenSource();
        }

        private double GetRealValue()
        {
            var cancel = _cancellation.Token;
            var wh = new AutoResetEvent(false);
            while (!wh.WaitOne(TimeSpan.FromMilliseconds(100)) && !cancel.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
            if (cancel.IsCancellationRequested)
                return default(double);
            NextStep();
            return RealValue;
        }

        private bool GetAccept()
        {
            var cancel = _cancellation.Token;
            var wh = new AutoResetEvent(false);
            TitleBtnNext = "Подтвердите или отмените результат калибровки";
            _doStep = () => wh.Set();
            while (!wh.WaitOne(TimeSpan.FromMilliseconds(100)) && !cancel.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
            if (cancel.IsCancellationRequested)
                return false;
            TitleBtnNext = "Старт";
            NextStep();
            return Accept;
        }
    }
}