using System;
using KipTM.Archive.ViewModel;

namespace KipTM.ViewModel.Workflow.States
{
    class ResultState : IWorkflowStep
    {
        private TestResultViewModel _result;
        private readonly TestResultViewModelFactory _checkFactory;

        public ResultState(TestResultViewModelFactory checkFactory)
        {
            _checkFactory = checkFactory;
        }

        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
        public void StateIn()
        {
             _result = _checkFactory.GetTestResult();
        }

        public void StateOut()
        {
            
        }

        public object ViewModel { get { return _result; } }
    }
}
