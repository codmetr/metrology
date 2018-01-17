using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using KipTM.Workflow;
using Tools.View;

namespace CheckFrame.Workflow
{
    /// <summary>
    /// Конвейер - механизм линейного перехода между состояниями
    /// </summary>
    public class LineWorkflow : INotifyPropertyChanged, IWorkflow, IDisposable
    {
        #region Поля

        /// <summary>
        /// Шаги
        /// </summary>
        private readonly List<IWorkflowStep> _states;

        /// <summary>
        /// Индекс текущего шага
        /// </summary>
        private int _index;

        /// <summary>
        /// Значение доступности следующего шага исходя из текущего индекса
        /// </summary>
        private bool _nextAvailableByIndex;

        /// <summary>
        /// Значение доступности следующего шага исходя из состояния текущего шага
        /// </summary>
        private bool _backAvailableByStep = true;

        /// <summary>
        /// Значение доступности предыдущего шага исходя из текущего индекса
        /// </summary>
        private bool _backAvailableByIndex;

        /// <summary>
        /// Значение доступности предыдущего шага исходя из состояния текущего шага
        /// </summary>
        private bool _nextAvailableByStep = true;

        /// <summary>
        /// Текущий шаг
        /// </summary>
        private IWorkflowStep _currentState;

        #endregion

        /// <summary>
        /// Конвейер - механизм линейного перехода между состояниями
        /// </summary>
        /// <param name="states">Набор доступных состояний</param>
        public LineWorkflow(List<IWorkflowStep> states)
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
            _nextAvailableByIndex = _index < _states.Count - 1;
            _backAvailableByIndex = _index > 0;

            OnPropertyChanged("NextAvailable");
            OnPropertyChanged("BackAvailable");
        }

        #region Implimentation IWorkflow

        public IWorkflowStep CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (_currentState != null)
                {
                    Detach(_currentState);
                    _currentState.StateOut();
                }
                _currentState = value;
                if (_currentState != null)
                {
                    Attach(_currentState);
                    _currentState.StateIn();
                }
                OnPropertyChanged();
            }
        }

        public bool NextAvailable
        {
            get { return _nextAvailableByIndex && _nextAvailableByStep; }
        }

        public bool BackAvailable
        {
            get { return _backAvailableByIndex && _backAvailableByStep; }
        }


        public ICommand Next
        {
            get { return new CommandWrapper(_next); }
        }

        public ICommand Back
        {
            get { return new CommandWrapper(_back); }
        }

        /// <summary>
        /// Шаги
        /// </summary>
        public IEnumerable<IWorkflowStep> States
        {
            get { return _states; }
        }

        #endregion

        #region Service

        private void _next()
        {
            if (_states == null)
                return;
            if (_index >= _states.Count - 1)
            {
                throw new IndexOutOfRangeException(string.Format("On next index {0} over count {1}", _index,
                    _states.Count));
            }
            _index++;
            CurrentState = _states[_index];
            _nextAvailableByIndex = _index < _states.Count - 1;
            _backAvailableByIndex = _index > 0;
            OnPropertyChanged("NextAvailable");
            OnPropertyChanged("BackAvailable");
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
            _nextAvailableByIndex = _index < _states.Count - 1;
            _backAvailableByIndex = _index > 0;
            OnPropertyChanged("NextAvailable");
            OnPropertyChanged("BackAvailable");
        }

        private void Attach(IWorkflowStep step)
        {
            step.NextAvailabilityChanged += step_NextAvailabilityChanged;
            step.BackAvailabilityChanged += step_BackAvailabilityChanged;
        }

        private void Detach(IWorkflowStep step)
        {
            step.NextAvailabilityChanged -= step_NextAvailabilityChanged;
            step.BackAvailabilityChanged -= step_BackAvailabilityChanged;
        }

        void step_NextAvailabilityChanged(object sender, WorkflowStepChangeEvent e)
        {
            _nextAvailableByStep = e.NewState;
            OnPropertyChanged("NextAvailable");
        }

        void step_BackAvailabilityChanged(object sender, WorkflowStepChangeEvent e)
        {
            _backAvailableByStep = e.NewState;
            OnPropertyChanged("BackAvailable");
        }

        #endregion

        #region INotifiPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_states != null)
                foreach (var step in _states)
                {
                    var dispStep = step.ViewModel as IDisposable;
                    if (dispStep != null)
                        dispStep.Dispose();
                }
        }

        #endregion

        #region IEnumerable<IWorkflowStep>

        private bool _isStart = true;

        public IEnumerator<IWorkflowStep> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        public bool MoveNext()
        {
            if (_isStart)
            {
                _isStart = false;
                return true;
            }

            if (!_nextAvailableByIndex)
                return false;
            _next();
            return true;
        }

        public void Reset()
        {
            while (_backAvailableByIndex)
                _back();
            _isStart = true;
        }

        public IWorkflowStep Current { get { return CurrentState; } }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
