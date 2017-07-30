using System;
using KipTM.Archive.ViewModel;
using KipTM.ViewModel;
using KipTM.ViewModel.Workflow;

namespace KipTM.Workflow.States
{
    /// <summary>
    /// Состояние просмотра результата
    /// </summary>
    class ResultState : IWorkflowStep
    {
        private TestResultViewModel _result;
        private readonly TestResultViewModelFactory _checkFactory;

        /// <summary>
        /// Состояние просмотра результата
        /// </summary>
        /// <param name="checkFactory"></param>
        public ResultState(TestResultViewModelFactory checkFactory)
        {
            _checkFactory = checkFactory;
        }

#pragma warning disable 0067
        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
#pragma warning restore 0067
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
