﻿using System;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Checks.ViewModel;
using KipTM.EventAggregator;
using KipTM.ViewModel.Checks;
using KipTM.ViewModel.Workflow;
using KipTM.Workflow.States.Events;

namespace KipTM.Workflow.States
{
    /// <summary>
    /// Состояние выполнения проверки
    /// </summary>
    public class CheckState : IWorkflowStep
    {
        private IMethodViewModel _check;
        private readonly CheckFactory _checkFactory;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Состояние выполнения проверки
        /// </summary>
        /// <param name="checkFactory"></param>
        /// <param name="eventAggregator"></param>
        public CheckState(CheckFactory checkFactory, IEventAggregator eventAggregator)
        {
            _checkFactory = checkFactory;
            _eventAggregator = eventAggregator;
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

        public object ViewModel { get { return _check; } }

        public void StateIn()
        {
            _check = _checkFactory.GetViewModelFor();
            if(_check != null)
                AttachEvents(_check);
        }

        public void StateOut()
        {
            if (_check != null)
            {
                _check.Cleanup();
                DetachEvents(_check);
            }
        }

        private void AttachEvents(IMethodViewModel check)
        {
            check.Started += check_Started;
            check.Stoped += check_Stoped;
        }

        private void DetachEvents(IMethodViewModel check)
        {
            check.Started -= check_Started;
            check.Stoped -= check_Stoped;
        }

        void check_Started(object sender, EventArgs e)
        {
            _eventAggregator.Send(new EventCheckState(true));
            OnNextAvailabilityChanged(new WorkflowStepChangeEvent(false));
            OnBackAvailabilityChanged(new WorkflowStepChangeEvent(false));
        }

        void check_Stoped(object sender, EventArgs e)
        {
            _eventAggregator.Send(new EventCheckState(false));
            OnNextAvailabilityChanged(new WorkflowStepChangeEvent(true));
            OnBackAvailabilityChanged(new WorkflowStepChangeEvent(true));
        }

    }
}
