using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KipTM.ViewModel.Master
{
    public class Workflow:INotifyPropertyChanged
    {
        private readonly List<IWorkflowStep> _states;
        private int _index;
        private bool _nextAvailable;
        private bool _backAvailable;
        private IWorkflowStep _currentState;

        public Workflow(List<IWorkflowStep> states)
        {
            _states = states;
            if(_states==null)
                return;
            _index = -1;
            if (_states.Count > 0)
                _index = 0;
            else
                return;
            CurrentState = _states[_index];
            NextAvailable = _index < _states.Count - 1;
            BackAvailable = _index > 0;
        }


        public IWorkflowStep CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (_currentState != null)
                    _currentState.StateOut();
                _currentState = value;
                if (_currentState != null)
                    _currentState.StateIn();
                OnPropertyChanged();
            }
        }

        public bool NextAvailable
        {
            get { return _nextAvailable; }
            set
            {
                _nextAvailable = value; 
                OnPropertyChanged();
            }
        }

        public bool BackAvailable
        {
            get { return _backAvailable; }
            set
            {
                _backAvailable = value; 
                OnPropertyChanged();
            }
        }


        public ICommand Next { get { return new CommandWrapper(_next); } }

        public ICommand Back { get { return new CommandWrapper(_back); } }

        private void _next()
        {
            if (_states == null)
                return;
            if (_index >= _states.Count - 1)
            {
                throw new IndexOutOfRangeException(string.Format("On next index {0} over count {1}", _index, _states.Count));
            }
            _index++;
            CurrentState = _states[_index];
            NextAvailable = _index < _states.Count - 1;
            BackAvailable = _index > 0;
        }

        private void _back()
        {
            if (_states == null)
                return;
            if (_index <= 0)
            {
                throw new IndexOutOfRangeException(string.Format("On back index {0} less or equal 0", _index));
            }

            _index--;
            CurrentState = _states[_index];
            NextAvailable = _index < _states.Count - 1;
            BackAvailable = _index > 0;
        }

        public void Attach(IWorkflowStep step)
        {
            step.NextAvailabilityChanged += step_NextAvailabilityChanged;
            step.BackAvailabilityChanged += step_BackAvailabilityChanged;
        }

        public void Detach(IWorkflowStep step)
        {
            step.NextAvailabilityChanged -= step_NextAvailabilityChanged;
            step.BackAvailabilityChanged -= step_BackAvailabilityChanged;
        }

        void step_NextAvailabilityChanged(object sender, WorkflowStepChangeEvent e)
        {
            NextAvailable |= e.NewState;
        }

        void step_BackAvailabilityChanged(object sender, WorkflowStepChangeEvent e)
        {
            BackAvailable |= e.NewState;
        }

        #region INotifiPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
