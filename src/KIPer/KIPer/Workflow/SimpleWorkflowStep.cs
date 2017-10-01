using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KipTM.Workflow
{
    class SimpleWorkflowStep:IWorkflowStep
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
    }
}
