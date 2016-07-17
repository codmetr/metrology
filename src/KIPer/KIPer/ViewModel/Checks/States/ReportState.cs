using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.ViewModel.Master;

namespace KipTM.ViewModel.Checks.States
{
    class ReportState : IWorkflowStep
    {
        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
        public void StateIn()
        {
            throw new NotImplementedException();
        }

        public void StateOut()
        {
            throw new NotImplementedException();
        }

        public object ViewModel { get; private set; }
    }
}
