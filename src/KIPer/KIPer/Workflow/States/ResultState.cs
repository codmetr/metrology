using System;
using KipTM.Archive.ViewModel;

namespace KipTM.ViewModel.Workflow.States
{
    class ResultState : IWorkflowStep
    {
        private TestResultViewModel _result;
        private readonly TestResultViewModelFactory _checkFabric;

        public ResultState(TestResultViewModelFactory checkFabric)
        {
            _checkFabric = checkFabric;
        }

        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
        public void StateIn()
        {
             _result = _checkFabric.GetTestResult();
        }

        public void StateOut()
        {
            
        }

        public object ViewModel { get { return _result; } }
    }
}
