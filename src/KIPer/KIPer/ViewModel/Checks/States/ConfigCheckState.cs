using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Checks;
using KipTM.ViewModel.Master;

namespace KipTM.ViewModel.Checks.States
{
    public class ConfigCheckState : IWorkflowStep
    {
        private CheckConfigViewModel _config;

        public ConfigCheckState(CheckConfigViewModel config)
        {
            _config = config;
        }

        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;

        protected virtual void OnNextAvailabilityChanged(WorkflowStepChangeEvent e)
        {
            EventHandler<WorkflowStepChangeEvent> handler = NextAvailabilityChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;

        protected virtual void OnBackAvailabilityChanged(WorkflowStepChangeEvent e)
        {
            EventHandler<WorkflowStepChangeEvent> handler = BackAvailabilityChanged;
            if (handler != null) handler(this, e);
        }

        public object ViewModel { get { return _config; } }

        public void StateIn()
        {
            
        }

        public void StateOut()
        {
            
        }

    }
}
