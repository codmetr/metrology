using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArchiveData;
using ArchiveData.DTO;
using CheckFrame.Workflow;
using DPI620Genii;
using KipTM.EventAggregator;
using KipTM.Workflow;
using NLog;
using PressureSensorCheck.Check;
using PressureSensorCheck.Devices;
using PressureSensorCheck.Report;
using PressureSensorCheck.Workflow.Content;
using PressureSensorData;
using Tools;

namespace PressureSensorCheck.Workflow
{
    public class PressureSensorWorkflow
    {
        public IWorkflow Make(Logger logger, IDataAccessor accessor, TestResultID id = null, PressureSensorResult result = null, PressureSensorConfig conf = null, IEventAggregator agregator = null)
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
            var configArchive = new TemplateArchive<PressureSensorConfig>();
            try
            {
                conf = configArchive.GetLast();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"On load config throwed exception: {e.ToString()}");
            }
            finally
            {
                if (conf == null)
                {
                    conf = PressureSensorConfig.GetDefault();
                    conf.CertificateNumber = Properties.Settings.Default.CertificateNumber.ToString();
                    conf.ReportNumber = Properties.Settings.Default.ReportNumber.ToString();
                }
            }

            var ports = System.IO.Ports.SerialPort.GetPortNames();
            var dpiConf = new DPI620GeniiConfig() {Ports = ports};
            if (!dpiConf.Ports.Contains(dpiConf.SelectPort))
                dpiConf.SelectPort = ports.FirstOrDefault();
            var res = new PressureSensorResult();

            var configVm = new PressureSensorCheckConfigVm(id, conf, dpiConf, agregator, configArchive);
            var dpiLog = NLog.LogManager.GetLogger("Dpi620");
            var dpiCom = new DPI620DriverCom().Setlog((msg) => dpiLog.Trace(msg));
            var dpi = AppVersionHelper.CurrentAppVersionType == AppVersionHelper.AppVersionType.Emulation?
                (IDPI620Driver)new DPI620Emulation():dpiCom;
            var run = new PressureSensorRunVm(conf, dpi, dpiConf, result, agregator);
            var resultVm = new PressureSensorResultVM(id, accessor, res, conf, agregator);

            var reportUpdater =new ReportUpdater();
            var certificateUpdater = new CertificateUpdater();
            var reportMain = new PressureSensorReportDto();
            var reportCertificate = new PressureSensorCertificateDto();
            var reportVm = new PressureSensorReportViewModel(reportMain, reportCertificate);
            var steps = new List<IWorkflowStep>()
            {
                new SimpleWorkflowStep(configVm).SetOut(()=>UpdateRunByConf(configVm.Config, run, logger)),
                new SimpleWorkflowStep(run).SetOut(()=>UpdateResultByRun(run, resultVm, logger)),
                new SimpleWorkflowStep(resultVm),
                new SimpleWorkflowStep(reportVm).SetIn(()=>
                {
                    reportUpdater.Update(id, conf, result, reportMain);
                    certificateUpdater.Update(id, conf, result, reportCertificate);
                }),
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
                        run.Points.Add(new PointViewModel() { Config = point, Result = new PointResultViewModel()});
                    else
                    {
                        resPoint.Config.Unit = point.Unit;
                        resPoint.Config.I = point.I;
                        resPoint.Config.dI = point.dI;
                        resPoint.Config.Ivar = point.Ivar;
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
                var i = 1;
                foreach (var point in run.Points)
                {
                    point.Index = i;
                    i++;
                    var resPoint = result.PointResults.FirstOrDefault(el => Math.Abs(el.Config.Pressure - point.Config.Pressure) < Double.Epsilon);
                    if(resPoint == null)
                        result.PointResults.Add(point);
                    else
                    {
                        resPoint.Config.Unit = point.Config.Unit;
                        resPoint.Config.I = point.Config.I;
                        resPoint.Config.dI = point.Config.dI;
                        resPoint.Config.Ivar = point.Config.Ivar;
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
    }
}
