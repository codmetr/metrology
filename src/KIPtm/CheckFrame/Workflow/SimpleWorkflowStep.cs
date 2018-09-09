using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KipTM.Workflow;

namespace CheckFrame.Workflow
{
    public class SimpleWorkflowStep:IWorkflowStep, INotifyPropertyChanged, IDisposable
    {
        private Action _stateIn = null;
        private Action _stateOut = null;
        private List<IDisposable> _disp = new List<IDisposable>();

        public SimpleWorkflowStep(object vm)
        {
            ViewModel = vm;
        }

        /// <summary>
        /// Дополнить действием входа в состояние
        /// </summary>
        /// <param name="inAction"></param>
        /// <returns></returns>
        public SimpleWorkflowStep SetIn(Action inAction)
        {
            _stateIn = inAction;
            return this;
        }

        /// <summary>
        /// Дополнить действием выхода из состояния
        /// </summary>
        /// <param name="outAction"></param>
        /// <returns></returns>
        public SimpleWorkflowStep SetOut(Action outAction)
        {
            _stateOut = outAction;
            return this;
        }

        /// <summary>
        /// Дополнить связанным разрушаемым объектом
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        public SimpleWorkflowStep AppendDisposable(IDisposable disposable)
        {
            _disp.Add(disposable);
            return this;
        }

        #region IWorkflowStep

        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
        public void StateIn()
        {
            _stateIn?.Invoke();
        }

        public void StateOut()
        {
            _stateOut?.Invoke();

        }

        public object ViewModel { get; }
        
        #endregion

        protected virtual void OnNextAvailabilityChanged(WorkflowStepChangeEvent e)
        {
            NextAvailabilityChanged?.Invoke(this, e);
        }

        protected virtual void OnBackAvailabilityChanged(WorkflowStepChangeEvent e)
        {
            BackAvailabilityChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposable

        public void Dispose()
        {
            _disp.ForEach(disp=>disp.Dispose());
            (ViewModel as IDisposable)?.Dispose();
        }

        #endregion
    }
}
