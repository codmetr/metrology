using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using ArchiveData;
using ArchiveData.DTO;
using CheckFrame.Workflow;
using DPI620Genii;
using KipTM.EventAggregator;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
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
        /// <summary>
        /// Создание стратегии переходов между экранами
        /// </summary>
        /// <param name="accessor">Доступ к хранилищу</param>
        /// <param name="context">Контекст UI</param>
        /// <param name="presSources">набор фабрик эталонных каналов</param>
        /// <param name="logger">логгер</param>
        /// <param name="id">ID проверки(если это продолжение проверки)</param>
        /// <param name="result">результат проверки(если это продолжение проверки)</param>
        /// <param name="conf">конфигурация проверки(если это продолжение проверки)</param>
        /// <param name="agregator">агрегатор событий</param>
        /// <returns>Стратегия переходов с состояниями</returns>
        public IWorkflow Make(IDataAccessor accessor, IContext context, IEnumerable<IEtalonSourceCannelFactory<Units>> presSources,
            Logger logger, TestResultID id = null, PressureSensorResult result = null, PressureSensorConfig conf = null, IEventAggregator agregator = null)
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
            var dpiConfVm = new DPI620GeniiConfigVm(context);
            var dpiConf = new DPI620GeniiConfig(dpiConfVm, ports);
            var res = new PressureSensorResult();

            //набор фабрик эталонных каналов
            var dictConf = new Dictionary<string, IEtalonSourceCannelFactory<Units>>();
            var userKey = "Без удаленного управления";//TODO локализовать
            dictConf.Add(userKey, null);
            foreach (var presSource in presSources)
            {
                dictConf.Add(presSource.TypeName, presSource);
            }
            var chLogicConfVm = new CheckPressureLogicConfigVm(context, conf);/*TODO reverse dependency*/
            var configVm = new PressureSensorCheckConfigVm(context, chLogicConfVm, dpiConfVm);
            var configurator = new PressureSensorCheckConfigurator(id, conf, dpiConf, configArchive, dictConf, configVm);

            var dpiLog = NLog.LogManager.GetLogger("Dpi620");
            var dpiCom = new DPI620DriverCom().Setlog((msg) => dpiLog.Trace(msg));
            var dpi = AppVersionHelper.CurrentAppVersionType == AppVersionHelper.AppVersionType.Emulation?
                (IDPI620Driver)new DPI620Emulation():dpiCom;
            //var run = new PressureSensorRunVm(conf, dpi, dpiConf, result, agregator);
            var runVm = new PressureSensorRunVm(conf.Unit, Units.mA, context);
            var runPresenter = new PressureSensorRunPresenter(runVm, conf, dpi, dpiConf, result, agregator, context);
            var resultVm = new PressureSensorResultVM(res, conf, context);
            var resultPresenter = new PressureSensorResultPresenter(id, accessor, res, conf, agregator, resultVm);

            var reportUpdater =new ReportUpdater();
            var certificateUpdater = new CertificateUpdater();
            var reportMain = new PressureSensorReportDto();
            var reportCertificate = new PressureSensorCertificateDto();
            var reportVm = new PressureSensorReportViewModel(reportMain, reportCertificate);
            var steps = new List<IWorkflowStep>()
            {
                new SimpleWorkflowStep(configVm).SetOut(()=>UpdateRunByConf(conf, dictConf[configVm.SelectedSourceName], runPresenter, logger)),
                new SimpleWorkflowStep(runVm).SetOut(()=>UpdateResultByRun(runPresenter, resultPresenter, logger)).AppendDisposable(runPresenter),
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
        /// <param name="etalonSourceFactory"></param>
        /// <param name="run">выполнение проверки</param>
        /// <param name="logger">логгер</param>
        private void UpdateRunByConf(PressureSensorConfig config, IEtalonSourceCannelFactory<Units> etalonSourceFactory, PressureSensorRunPresenter run, Logger logger)
        {
            try
            {
                if (run.IsRun)
                    return;
                run.UpdateSourceChannel(etalonSourceFactory?.GetChanel());
                run.UpdatePoint(config.Points);
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
        private void UpdateResultByRun(PressureSensorRunPresenter run, PressureSensorResultPresenter result, Logger logger)
        {
            //if(run.IsRun)
            //    return;

            try
            {
                result.SetResult(run.Result);
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
