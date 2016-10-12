using System;

namespace KipTM.ViewModel.Workflow
{
    public interface IWorkflowStep
    {
        /// <summary>
        /// Новое состояние кнопки вперед
        /// </summary>
        event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;

        /// <summary>
        /// Новое состояние кнопки назад
        /// </summary>
        event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;

        /// <summary>
        /// Состояние актиаировано
        /// </summary>
        void StateIn();

        /// <summary>
        /// Состояние деактивировано
        /// </summary>
        void StateOut();

        object ViewModel { get; }
    }
}