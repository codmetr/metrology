using System;
using System.Collections.Generic;
using System.Linq;
using ArchiveData;
using ArchiveData.DTO;
using CheckFrame.Workflow;
using DPI620Genii;
using KipTM.Workflow;
using Moq;
using NLog;
using PressureSensorCheck.Check;
using PressureSensorCheck.Report;
using PressureSensorData;

namespace PressureSensorCheck.Workflow
{
    public class PressureSensorWorkflow
    {
        public IWorkflow Make(Logger logger, IDataAccessor accessor, TestResultID id = null, PressureSensorResult result = null, PressureSensorConfig conf = null)
        {
            id = id ?? new TestResultID()
            {
                TargetDeviceKey = PresSensorCheck.CheckKey,
                DeviceType = PresSensorCheck.CheckName,
            };
            result = result ?? new PressureSensorResult()
            {
                Assay = "корректно",
                Leak = "герметичен",
                CommonResult = "пригоден",
                VisualCheckResult = "без видимых дефектов",
            };
            if (conf == null)
            {
                conf = PressureSensorConfig.GetDefault();
                conf.CertificateNumber = Properties.Settings.Default.CertificateNumber.ToString();
                conf.ReportNumber = Properties.Settings.Default.ReportNumber.ToString();
            }

            var ports = System.IO.Ports.SerialPort.GetPortNames();
            var dpiConf = new DPI620GeniiConfig() {Ports = ports};
            if (!dpiConf.Ports.Contains(dpiConf.SelectPort))
                dpiConf.SelectPort = ports.FirstOrDefault();
            var res = new PressureSensorResult();

            var configVm = new PressureSensorCheckConfigVm(id, conf, dpiConf);
            var run = new PressureSensorRunVm(conf, new DPI620DriverCom(), dpiConf, result);
            var resultVm = new PressureSensorResultVM(id, accessor, res, conf);

            var reportUpdater =new ReportUpdater();
            var certificateUpdater = new CertificateUpdater();
            var reportMain = new PressureSensorReportDto();
            var reportCertificate = new PressureSensorCertificateDto();

            var steps = new List<IWorkflowStep>()
            {
                new SimpleWorkflowStep(configVm).SetOut(()=>UpdateRunByConf(configVm.Config, run, logger)),
                new SimpleWorkflowStep(run).SetOut(()=>UpdateResultByRun(run, resultVm, logger)),
                new SimpleWorkflowStep(resultVm).SetOut(()=>
                {
                    reportUpdater.Update(conf, result, reportMain);
                    certificateUpdater.Update(id, conf, result, reportCertificate);
                }),
                new SimpleWorkflowStep(new PressureSensorReportViewModel(reportMain, reportCertificate)),
            };

            return new LineWorkflow(steps);
        }

        /// <summary>
        /// Обновление визуальной модели выполнения по конфигурации
        /// </summary>
        /// <param name="config">конфигурация</param>
        /// <param name="run">выполнение проверки</param>
        /// <param name="logger">логгер</param>
        private void UpdateRunByConf(CheckPressureLogicConfigVm config, PressureSensorRunVm run, Logger logger)
        {
            try
            {
                if (run.IsRun)
                    return;
                run.Points.Clear();
                foreach (var point in config.Points)
                {
                    var resPoint = run.Points.FirstOrDefault(el => Math.Abs(el.Config.Pressure - point.Pressure) < Double.Epsilon);
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

        /// <summary>
        /// Обновление модели результата по текущему прогрессу выполненния проверки
        /// </summary>
        /// <param name="run">проверка</param>
        /// <param name="result">результат</param>
        /// <param name="logger">логгер</param>
        private void UpdateResultByRun(PressureSensorRunVm run, PressureSensorResultVM result, Logger logger)
        {
            //if(run.IsRun)
            //    return;

            try
            {
                result.Data = run.Result;
                result.PointResults.Clear();
                foreach (var point in run.Points)
                {
                    var resPoint = result.PointResults.FirstOrDefault(el => Math.Abs(el.Config.Pressure - point.Config.Pressure) < Double.Epsilon);
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
                result.TimeStamp = run.LastResultTime;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                Console.WriteLine(e);
                throw;
            }
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
