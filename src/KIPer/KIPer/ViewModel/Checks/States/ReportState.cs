using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.ViewModel.Master;
using KipTM.ViewModel.Report;

namespace KipTM.ViewModel.Checks.States
{
    class ReportState : IWorkflowStep
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
            _report = _reportFabric();
        }

        public void StateOut()
        {
            
        }

        public object ViewModel { get { return _report; } }
    }
}
