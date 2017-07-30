using System;

namespace KipTM.Workflow
{
    /// <summary>
    /// Новое состояние доступности перехода
    /// </summary>
    public class WorkflowStepChangeEvent:EventArgs
    {
        /// <summary>
        /// Новое состояние доступности перехода
        /// </summary>
        /// <param name="newState">Новое состояние доступности перехода</param>
        public WorkflowStepChangeEvent(bool newState)
        {
            NewState = newState;
        }

        /// <summary>
        /// Новое состояние доступности перехода
        /// </summary>
        public bool NewState { get; private set; }
    }
}
