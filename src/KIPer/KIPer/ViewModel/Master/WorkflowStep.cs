using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.Master
{
    /// <summary>
    /// Описатель состояния
    /// </summary>
    public abstract class WorkflowStep
    {
        /// <summary>
        /// Новое состояние кнопки вперед
        /// </summary>
        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;

        protected virtual void OnNextAvailabilityChanged(WorkflowStepChangeEvent e)
        {
            var handler = NextAvailabilityChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Новое состояние кнопки назад
        /// </summary>
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;

        protected virtual void OnBackAvailabilityChanged(WorkflowStepChangeEvent e)
        {
            var handler = BackAvailabilityChanged;
            if (handler != null) handler(this, e);
        }


    }
}
