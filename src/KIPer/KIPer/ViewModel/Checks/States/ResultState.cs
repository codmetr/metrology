using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.ViewModel.Master;

namespace KipTM.ViewModel.Checks.States
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
