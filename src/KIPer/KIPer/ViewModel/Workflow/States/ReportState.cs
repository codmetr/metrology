using System;
using KipTM.ViewModel.Report;

namespace KipTM.ViewModel.Workflow.States
{
    class ReportState : IWorkflowStep, IDisposable
    {
        private IReportViewModel _report;
        private readonly Func<IReportViewModel> _reportFabric;

        public ReportState(Func<IReportViewModel> reportFabric)
        {
            _reportFabric = reportFabric;
        }


        public event EventHandler<WorkflowStepChangeEvent> NextAvailabilityChanged;
        public event EventHandler<WorkflowStepChangeEvent> BackAvailabilityChanged;
        public void StateIn()
        {
            var reportDispose = _report as IDisposable;
            if (reportDispose != null)
                reportDispose.Dispose();
            _report = _reportFabric();
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
