using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Checks.ViewModel.Config;

namespace KipTM.Workflow.States.PressureSensor
{
    class ResultState : IWorkflowStep
    {
        private PressureSensorResultVM _vm;

        public ResultState(PressureSensorResultVM vm)
        {
            _vm = vm;
        }

        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
        public void StateIn()
        {
            //throw new NotImplementedException();
        }

        public void StateOut()
        {
            //throw new NotImplementedException();
        }

        public object ViewModel => _vm;
    }
}
