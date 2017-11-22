using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPI620Genii;
using KipTM.Checks.ViewModel.Config;
using KipTM.Report.PressureSensor;
using KipTM.ViewModel.Checks.Config;
using KipTM.ViewModel.Workflow.States;
using Moq;
using NLog;

namespace KipTM.Workflow.States.PressureSensor
{
    public class PressureSensorWorkflow
    {
        public IWorkflow Make(Logger logger)
        {
            var configData = new CheckPressureSensorConfig();
            var ports = System.IO.Ports.SerialPort.GetPortNames();
            var dpiConf = new DPI620GeniiConfig()
            {
                Ports = ports,
                SelectPort = ports.FirstOrDefault(),
            };
            var config = new PressureSensorCheckConfigVm()
            {
                Config = configData,
                DpiConfig = dpiConf,
            };
            var run = new PressureSensorRunVm(configData, new DPI620DriverCom(), dpiConf);
            var result = new PressureSensorResultVM();
            var reportMain = new PressureSensorReportDto()
            {
                ReportNumber = "1",
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
                Ethalons = new[]
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
                MainAccurancy = new[]
                {
                    new MainAccurancyPointDto() {PressurePoint = "0", Uet = "5", U = "5.5", dU = "-0.5", dUet = "0.1"},
                    new MainAccurancyPointDto() {PressurePoint = "10", Uet = "4", U = "4.5", dU = "-0.5", dUet = "0.1"},
                    new MainAccurancyPointDto() {PressurePoint = "20", Uet = "3", U = "3.5", dU = "-0.5", dUet = "0.1"},
                    new MainAccurancyPointDto() {PressurePoint = "30", Uet = "2", U = "2.5", dU = "-0.5", dUet = "0.1"},
                    new MainAccurancyPointDto() {PressurePoint = "40", Uet = "1", U = "1.5", dU = "-0.5", dUet = "0.1"},
                    new MainAccurancyPointDto() {PressurePoint = "50", Uet = "0", U = "0.5", dU = "-0.5", dUet = "0.1"},
                },
                VariationAccurancy = new[]
                {
                    new VariationAccurancyPointDto() {PressurePoint = "0", Uf = "5", Ur = "5.5", dU = "0.5", dUet = "0.1"},
                    new VariationAccurancyPointDto() {PressurePoint = "10", Uf = "4", Ur = "4.5", dU = "0.5", dUet = "0.1"},
                    new VariationAccurancyPointDto() {PressurePoint = "20", Uf = "3", Ur = "3.5", dU = "0.5", dUet = "0.1"},
                    new VariationAccurancyPointDto() {PressurePoint = "30", Uf = "2", Ur = "2.5", dU = "0.5", dUet = "0.1"},
                    new VariationAccurancyPointDto() {PressurePoint = "40", Uf = "1", Ur = "1.5", dU = "0.5", dUet = "0.1"},
                    new VariationAccurancyPointDto() {PressurePoint = "50", Uf = "0", Ur = "0.5", dU = "0.5", dUet = "0.1"},
                }};

                var reportCertificate = new PressureSensorCertificateDto()
                {
                    Ethalons = new[]
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
                };
            var steps = new List<IWorkflowStep>()
            {
                new SimpleWorkflowStep(config).SetOut(()=>UpdateRunByConf(config, run, logger)),
                new SimpleWorkflowStep(run).SetOut(()=>UpdateResultByRun(run, result, logger)),
                new SimpleWorkflowStep(result).SetOut(()=>UpdateReportByResult(config, result, reportMain, reportCertificate)),
                new SimpleWorkflowStep(new PressureSensorReportViewModel(reportMain, reportCertificate)),
            };

            return new LineWorkflow(steps);
        }

        private void UpdateRunByConf(PressureSensorCheckConfigVm config, PressureSensorRunVm run, Logger logger)
        {

            try
            {
                if (run.IsRun)
                    return;
                run.Points.Clear();
                foreach (var point in config.Config.Points)
                {
                    var resPoint = run.Points.FirstOrDefault(el => Math.Abs(el.Config.Pressire - point.Pressire) < Double.Epsilon);
                    if (resPoint == null)
                        run.Points.Add(new PointViewModel() { Config = point});
                    else
                    {
                        resPoint.Config.Unit = point.Unit;
                        resPoint.Config.U = point.U;
                        resPoint.Config.dU = point.dU;
                        resPoint.Config.Uvar = point.Uvar;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                Console.WriteLine(e);
                throw;
            }
        }

        private void UpdateResultByRun(PressureSensorRunVm run, PressureSensorResultVM result, Logger logger)
        {
            //if(run.IsRun)
            //    return;

            try
            {
                result.PointResults.Clear();
                foreach (var point in run.Points)
                {
                    var resPoint = result.PointResults.FirstOrDefault(el => Math.Abs(el.Config.Pressire - point.Config.Pressire) < Double.Epsilon);
                    if(resPoint == null)
                        result.PointResults.Add(point);
                    else
                    {
                        resPoint.Config.Unit = point.Config.Unit;
                        resPoint.Config.U = point.Config.U;
                        resPoint.Config.dU = point.Config.dU;
                        resPoint.Config.Uvar = point.Config.Uvar;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                Console.WriteLine(e);
                throw;
            }
        }

        private void UpdateReportByResult(PressureSensorCheckConfigVm config, PressureSensorResultVM result, PressureSensorReportDto reportMain, PressureSensorCertificateDto reportCertificate)
        {
            reportMain.Assay = result.Assay;
            reportMain.CommonResult = result.CommonResult;
            reportMain.CertificateDate = DateTime.Now;//TODO: заполнить дату проверки из последнего изменения при прохождении проверки


            // Заполнение результатов проверки основной погрешности
            var mainAccur = (reportMain.MainAccurancy ?? new List<MainAccurancyPointDto>()).ToList();
            foreach (var point in result.PointResults)
            {
                var mainAcPoint = mainAccur.FirstOrDefault(
                        el => el.PressurePoint.ToString() == point.Config.Pressire.ToString("F0"));
                if (mainAcPoint == null)
                {
                    mainAcPoint = new MainAccurancyPointDto()
                    {
                        PressurePoint = point.Config.Pressire.ToString("F0"),
                        Uet = point.Config.U.ToString("F3"),
                        dUet = point.Config.dU.ToString("F3"),
                    };
                    mainAccur.Add(mainAcPoint);
                }
                mainAcPoint.U = point.Result.UReal.ToString("F3");
                mainAcPoint.dU = point.Result.dUReal.ToString("F3");
            }
            reportMain.MainAccurancy = mainAccur.OrderBy(el => int.Parse(el.PressurePoint.ToString())).ToArray();

            // Заполнение результатов проверки вариации
            var varAccur = (reportMain.VariationAccurancy ?? new List<VariationAccurancyPointDto>()).ToList();
            foreach (var point in result.PointResults)
            {
                var varAcPoint = varAccur.FirstOrDefault(
                    el => el.PressurePoint.ToString() == point.Config.Pressire.ToString("F0"));
                if (varAcPoint == null)
                {
                    varAcPoint = new VariationAccurancyPointDto()
                    {
                        PressurePoint = point.Config.Pressire.ToString("F0"),
                        dUet = point.Config.Uvar.ToString("F3"),
                    };
                    varAccur.Add(varAcPoint);
                }
                varAcPoint.dU = point.Result.Uvar.ToString("F3");
                varAcPoint.Uf = point.Result.UReal.ToString("F3");
                varAcPoint.Ur = point.Result.Uback.ToString("F3");
            }
            reportMain.VariationAccurancy = varAccur.OrderBy(el => int.Parse(el.PressurePoint.ToString())).ToArray();
        }

        private static IDPI620Driver GetMoq()
        {
            var moq = new Moq.Mock<IDPI620Driver>();

            moq.Setup(drv => drv.Open()).Callback(() => { Dpi620StateMoq.Instance.Start(); });
            moq.Setup(drv => drv.Close()).Callback(() => { Dpi620StateMoq.Instance.Stop(); });

            //moq.Setup(drv => drv.SetUnits(It.IsAny<int>(), It.IsAny<string>())).Callback(
            //    (int slotId, string unitCode) => { Dpi620StateMoq.Instance.SetUnit(slotId, unitCode); });

            moq.Setup(drv => drv.GetValue(It.IsAny<int>()))
                .Returns((int slotId) => Dpi620StateMoq.Instance.GetValue(slotId));
            return moq.Object;
        }

    }
}
