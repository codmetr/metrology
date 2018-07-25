using System;
using KipTM.ViewModel.Report;

namespace KipTM.Workflow.States
{
    /// <summary>
    /// Состояние просмотра отчета
    /// </summary>
    class ReportState : IWorkflowStep, IDisposable
    {
        private IReportViewModel _report;
        private readonly Func<IReportViewModel> _reportFactory;

        /// <summary>
        /// Состояние просмотра отчета
        /// </summary>
        /// <param name="reportFactory"></param>
        public ReportState(Func<IReportViewModel> reportFactory)
        {
            _reportFactory = reportFactory;
        }

#pragma warning disable 0067
        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
#pragma warning restore 0067
        public void StateIn()
        {
            var reportDispose = _report as IDisposable;
            if (reportDispose != null)
                reportDispose.Dispose();
            _report = _reportFactory();
        }

        public void StateOut()
        {
            
        }

        public object ViewModel { get { return _report; } }

        public void Dispose()
        {
            var reportDispose = _report as IDisposable;
            if (reportDispose != null)
                reportDispose.Dispose();
        }
    }
}
