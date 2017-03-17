using System;
using KipTM.ViewModel.Report;

namespace KipTM.ViewModel.Workflow.States
{
    class ReportState : IWorkflowStep, IDisposable
    {
        private IReportViewModel _report;
        private readonly Func<IReportViewModel> _reportFactory;

        public ReportState(Func<IReportViewModel> reportFactory)
        {
            _reportFactory = reportFactory;
        }


        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
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
