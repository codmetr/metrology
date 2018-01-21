using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KipTM.Workflow;

namespace CheckFrame.Workflow
{
    public class SimpleWorkflowStep:IWorkflowStep, INotifyPropertyChanged
    {
        private Action _stateIn = null;
        private Action _stateOut = null;

        public SimpleWorkflowStep(object vm)
        {
            ViewModel = vm;
        }

        public SimpleWorkflowStep SetIn(Action inAction)
        {
            _stateIn = inAction;
            return this;
        }

        public SimpleWorkflowStep SetOut(Action outAction)
        {
            _stateOut = outAction;
            return this;
        }

        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
        public void StateIn()
        {
            _stateIn?.Invoke();
        }

        public void StateOut()
        {
            _stateOut?.Invoke();

        }

        public object ViewModel { get; }

        protected virtual void OnNextAvailabilityChanged(WorkflowStepChangeEvent e)
        {
            NextAvailabilityChanged?.Invoke(this, e);
        }

        protected virtual void OnBackAvailabilityChanged(WorkflowStepChangeEvent e)
        {
            BackAvailabilityChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
