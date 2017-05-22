using System.Windows.Input;

namespace KipTM.ViewModel.Workflow
{
    public interface IWorkflow
    {
        /// <summary>
        /// Перейти к предыдущему шагу
        /// </summary>
        ICommand Back { get; }
        /// <summary>
        /// Доступность следующего шага
        /// </summary>
        bool BackAvailable { get; }
        /// <summary>
        /// Текущий шаг
        /// </summary>
        IWorkflowStep CurrentState { get; }
        /// <summary>
        /// Перейти к следующему шагу
        /// </summary>
        ICommand Next { get; }
        /// <summary>
        /// Доступности предыдущего
        /// </summary>
        bool NextAvailable { get; }
    }
}