using System;
using KipTM.ViewModel.Checks.Config;
using KipTM.Workflow;

namespace KipTM.ViewModel.Workflow.States
{
    /// <summary>
    /// Состояние конфигурации проверки
    /// </summary>
    public class ConfigCheckState : IWorkflowStep
    {
        private CheckConfigViewModel _config;

        /// <summary>
        /// Состояние конфигурации проверки
        /// </summary>
        /// <param name="config"></param>
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
