using GalaSoft.MvvmLight;
using KipTM.Model.Checks;

namespace KipTM.ViewModel.Checks
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class StepViewModel : ViewModelBase
    {
        private readonly ITestStep _step;
        private string _title;
        private double _progress;
        private string _note;
        private StepState _state;

        /// <summary>
        /// Initializes a new instance of the StepViewModel class.
        /// </summary>
        public StepViewModel(ITestStep step)
        {
            _step = step;
            Title = _step.Name;
            Progress = 0.0;
            _state = StepState.Base;
            Note = string.Empty;

            _step.Error += _step_Error;
            _step.ProgressChanged += _step_ProgressChanged;
        }

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public double Progress
        {
            get { return _progress; }
            set { Set(ref _progress, value); }
        }

        public int State
        {
            get { return (int)_state; }
            set { Set(ref _state, (StepState)value); }
        }

        public string Note
        {
            get { return _note; }
            set { Set(ref _note, value); }
        }

        void _step_ProgressChanged(object sender, EventArgProgress e)
        {
            if (e.Progress != null) Progress = e.Progress.Value;
            Note = e.Note;
        }

        void _step_Error(object sender, EventArgError e)
        {
            _state = StepState.Error;
            Note = e.ErrorString;
        }
    }
}