using System.Windows.Input;

namespace KipTM.Workflow
{
    /// <summary>
    /// Механизм перехода между состояниями
    /// </summary>
    public interface IWorkflow
    {
        /// <summary>
        /// Перейти к предыдущему состоянию
        /// </summary>
        ICommand Back { get; }
        /// <summary>
        /// Доступность следующего состояния
        /// </summary>
        bool BackAvailable { get; }
        /// <summary>
        /// Текущиее состояние
        /// </summary>
        IWorkflowStep CurrentState { get; }
        /// <summary>
        /// Перейти к следующему состоянию
        /// </summary>
        ICommand Next { get; }
        /// <summary>
        /// Доступности предыдущего состояния
        /// </summary>
        bool NextAvailable { get; }
    }
}