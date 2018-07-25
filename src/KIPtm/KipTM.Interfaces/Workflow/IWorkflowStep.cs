using System;

namespace KipTM.Workflow
{
    /// <summary>
    /// Модель состояния
    /// </summary>
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
        /// Состояние активировано
        /// </summary>
        void StateIn();

        /// <summary>
        /// Состояние деактивировано
        /// </summary>
        void StateOut();

        /// <summary>
        /// Визуальная модель
        /// </summary>
        object ViewModel { get; }
    }
}