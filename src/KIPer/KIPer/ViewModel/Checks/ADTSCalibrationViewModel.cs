using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using KipTM.Model.Checks;
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
        private bool _waitUserReaction;

        private Action _doStep = () => { };
        private double _realValue;

        private CancellationTokenSource _cancellation;
        private bool _accept;

        /// <summary>
        /// Initializes a new instance of the ADTSCalibrationViewModel class.
        /// </summary>
        public ADTSCalibrationViewModel(ADTSCheckMethodic methodic)
        {
            _cancellation = new CancellationTokenSource();
            _methodic = methodic;
            _methodic.SetFuncGetValue(GetRealValue);
            Points = new List<PointCheckableViewModel>(methodic.Steps.Select(el=>new PointCheckableViewModel(el.Name)));
            TitleBtnNext = "Старт";
            _doStep = Start;
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

        public ICommand StepCommand { get{return new RelayCommand(_doStep);} }

        public ObservableCollection<IParameterResultViewModel> Results { get; set; }

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

        private void Start()
        {
            TitleBtnNext = "Далее";
            _doStep = NextStep;
        }

        private void NextStep()
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
            _doStep = () => wh.Set();
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