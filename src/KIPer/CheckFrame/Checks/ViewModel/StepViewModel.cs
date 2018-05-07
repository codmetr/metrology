using System.ComponentModel;
using System.Runtime.CompilerServices;
using KipTM.Model.Checks;

namespace CheckFrame.ViewModel.Checks
{
    public class StepViewModel : INotifyPropertyChanged
    {
        private readonly ITestStep _step;
        private string _title;
        private double _progress;
        private string _note;
        private StepState _state;
        private bool _isEnabled;

        /// <summary>
        /// Initializes a new instance of the StepViewModel class.
        /// </summary>
        public StepViewModel(ITestStep step, bool isEnabled)
        {
            _step = step;
            _isEnabled = isEnabled;
            Title = _step.Name;
            Progress = 0.0;
            _state = StepState.Base;
            Note = string.Empty;

            _step.Started += _step_Started;
            _step.End += _step_End;
            _step.Error += _step_Error;
            _step.ProgressChanged += _step_ProgressChanged;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get { return _progress; }
            set { _progress = value;
                OnPropertyChanged();
            }
        }

        public int State
        {
            get { return (int)_state; }
            set { _state = (StepState)value;
                OnPropertyChanged();
            }
        }

        public string Note
        {
            get { return _note; }
            set { _note = value;
                OnPropertyChanged();
            }
        }

        void _step_ProgressChanged(object sender, EventArgProgress e)
        {
            if (e.Progress != null) Progress = e.Progress.Value;
            Note = e.Note;
        }

        void _step_Error(object sender, EventArgError e)
        {
            State = (int)StepState.Error;
            Note = e.ErrorString;
        }

        void _step_End(object sender, EventArgEnd e)
        {
            State = e.Result ? (int)StepState.Ok : (int)StepState.Error;
            Note = "";
        }

        void _step_Started(object sender, System.EventArgs e)
        {
            State = (int)StepState.Run;
            Note = "";
        }

        public virtual void Cleanup()
        {
            _step.Started -= _step_Started;
            _step.End -= _step_End;
            _step.Error -= _step_Error;
            _step.ProgressChanged -= _step_ProgressChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}