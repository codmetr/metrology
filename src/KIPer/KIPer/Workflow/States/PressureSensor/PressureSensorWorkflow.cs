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
                new ConfigPointsState(new PressureSensorPointsConfigVm(null, null)),
                new ResultState(new PressureSensorResultVM()),
                new ReportState(new PressureSensorReportViewModel(new PressureSensorReportDto()
                {
                    ReportNumber = "007",
                    ReportTime = "700",
                    TypeDevice = "123",
                    Assay = "Корректно",
                    CertificateDate = "01.01.0001",
                    CertificateNumber = "111",
                    CommonResult = "Ok",
                    Humidity = "50",
                    LeakCheckResult = "no leaks",
                    Owner = "я",
                    Pressure = "760",
                    SerialNumber = "ser1",
                    Temperature = "21",
                    User = "Иванов Иван Иванович",
                    VisualCheckResult = "не нашел",
                    Voltage = "220",
                    Ethalons = new []
                    {
                        new EthalonDto()
                        {
                            Title = "DPI620Genii",
                            Type = "многофункциональный манометр",
                            RangeClass = "0.001 ВПИ",
                            SerialNumber = "321",
                            CheckCertificateDate = "02.01.0001",
                            CheckCertificateNumber = "222"
                        }, 
                    },
                    MainAccurancy = new []
                    {
                        new MainAccurancyPointDto() {PressurePoint = "0", Uet = "5", U = "5.5", dU = "-0.5", dUet = "0.1"},
                        new MainAccurancyPointDto() {PressurePoint = "10", Uet = "4", U = "4.5", dU = "-0.5", dUet = "0.1"},
                        new MainAccurancyPointDto() {PressurePoint = "20", Uet = "3", U = "3.5", dU = "-0.5", dUet = "0.1"},
                        new MainAccurancyPointDto() {PressurePoint = "30", Uet = "2", U = "2.5", dU = "-0.5", dUet = "0.1"},
                        new MainAccurancyPointDto() {PressurePoint = "40", Uet = "1", U = "1.5", dU = "-0.5", dUet = "0.1"},
                        new MainAccurancyPointDto() {PressurePoint = "50", Uet = "0", U = "0.5", dU = "-0.5", dUet = "0.1"},
                    },
                    VariationAccurancy = new []
                    {
                        new VariationAccurancyPointDto() {PressurePoint = "0", Uf = "5", Ur = "5.5", dU = "0.5", dUet = "0.1"},
                        new VariationAccurancyPointDto() {PressurePoint = "10", Uf = "4", Ur = "4.5", dU = "0.5", dUet = "0.1"},
                        new VariationAccurancyPointDto() {PressurePoint = "20", Uf = "3", Ur = "3.5", dU = "0.5", dUet = "0.1"},
                        new VariationAccurancyPointDto() {PressurePoint = "30", Uf = "2", Ur = "2.5", dU = "0.5", dUet = "0.1"},
                        new VariationAccurancyPointDto() {PressurePoint = "40", Uf = "1", Ur = "1.5", dU = "0.5", dUet = "0.1"},
                        new VariationAccurancyPointDto() {PressurePoint = "50", Uf = "0", Ur = "0.5", dU = "0.5", dUet = "0.1"}, 
                    }
                },
                
                new PressureSensorCertificateDto()
                {
                    Ethalons = new []
                    {
                        new EthalonDto()
                        {
                            Title = "DPI620Genii",
                            Type = "многофункциональный манометр",
                            RangeClass = "0.001 ВПИ",
                            SerialNumber = "321",
                            CheckCertificateDate = "02.01.0001",
                            CheckCertificateNumber = "222"
                        },
                    },
                })),
            };

            return new LineWorkflow(steps);
        }
    }
}
