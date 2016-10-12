using System;

namespace KipTM.ViewModel.Workflow.States
{
    class ResultState : IWorkflowStep
    {
        private TestResultViewModel _result;
        private readonly Func<TestResultViewModel> _checkFabric;

        public ResultState(Func<TestResultViewModel> checkFabric)
        {
            _checkFabric = checkFabric;
        }

        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
        public void StateIn()
        {
             _result = _checkFabric();
        }

        public void StateOut()
        {
            
        }

        public object ViewModel { get { return _result; } }
    }
}
