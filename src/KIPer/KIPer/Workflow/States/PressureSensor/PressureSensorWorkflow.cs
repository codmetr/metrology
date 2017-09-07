using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Checks.ViewModel.Config;
using KipTM.Report.PressureSensor;
using KipTM.ViewModel.Checks.Config;
using KipTM.ViewModel.Workflow.States;

namespace KipTM.Workflow.States.PressureSensor
{
    public class PressureSensorWorkflow
    {
        public IWorkflow Make()
        {
            var steps = new List<IWorkflowStep>()
            {
                new ConfigState(new PressureSensorCheckConfigVm()),
                new ConfigPointsState(new PressureSensorPointsConfigVm()),
                new ResultState(new PressureSensorResultVM()),
                new ReportState(new PressureSensorReportViewModel(new PressureSensorReportDto()
                {
                    ReportNumber = "007",
                    ReportTime = "700",
                    TypeDevice = "123"
                })),
            };

            return new LineWorkflow(steps);
        }
    }
}
