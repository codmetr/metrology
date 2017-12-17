﻿using System;
using System.Collections.Generic;
using System.Linq;
using DPI620Genii;
using KipTM.Report.PressureSensor;
using KipTM.Workflow;
using Moq;
using NLog;

namespace PressureSensorCheck.Workflow
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
            var result = new PressureSensorResultVM(config);
            var reportMain = new PressureSensorReportDto()
            {
                ReportNumber = "1",
                ReportTime = "700",
                TypeDevice = "",
                Assay = "Корректно",
                CertificateDate = DateTime.Now.ToString("dd.MM.yyyy"),
                CertificateNumber = "",
                CommonResult = "Ok",
                Humidity = "50",
                LeakCheckResult = "нет утечек",
                Owner = "",
                Pressure = "760",
                SerialNumber = "",
                Temperature = "21",
                User = "",
                VisualCheckResult = "не нашел",
                Voltage = "220",
                Ethalons = new[]
                {
                    new EthalonDto()
                    {
                        Title = "DPI620Genii",
                        Type = "многофункциональный манометр",
                        RangeClass = "0.001 ВПИ",
                        SerialNumber = "",
                        CheckCertificateDate = DateTime.Now.ToString("dd.MM.yyyy"),
                        CheckCertificateNumber = ""
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
                }
            };

            var reportCertificate = new PressureSensorCertificateDto()
            {
                Ethalons = new[]
                {
                    new EthalonDto()
                    {
                        Title = "DPI620Genii",
                        Type = "многофункциональный манометр",
                        RangeClass = "0.001 ВПИ",
                        SerialNumber = "",
                        CheckCertificateDate = DateTime.Now.ToString("dd.MM.yyyy"),
                        CheckCertificateNumber = ""
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

        /// <summary>
        /// Обновление визуальной модели выполнения по конфигурации
        /// </summary>
        /// <param name="config">конфигурация</param>
        /// <param name="run">выполнение проверки</param>
        /// <param name="logger">логгер</param>
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
                result.LastResult = run.LastResult;
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
                result.TimeStamp = run.LastResultTime;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Обновление отчета по результату
        /// </summary>
        /// <param name="config">конфигурация</param>
        /// <param name="result">результат</param>
        /// <param name="reportMain">отчет</param>
        /// <param name="reportCertificate">сервификат</param>
        private void UpdateReportByResult(PressureSensorCheckConfigVm config, PressureSensorResultVM result, PressureSensorReportDto reportMain, PressureSensorCertificateDto reportCertificate)
        {
            ApplyCommonData(config, result, reportMain);

            ApplyEthalons(config, reportMain);

            // Заполнение результатов проверки основной погрешности
            ApplyMainAccurancy(result, reportMain);

            // Заполнение результатов проверки вариации
            ApplyVariation(result, reportMain);
        }

        private void ApplyEthalons(PressureSensorCheckConfigVm config, PressureSensorReportDto reportMain)
        {
            var ethalons = new List<EthalonDto>();
            ethalons.Add(new EthalonDto()
            {
                Type = config.EthalonPressure.SensorType,
                Title = config.EthalonPressure.Title,
                RangeClass = config.EthalonPressure.ErrorClass,
                SerialNumber = config.EthalonPressure.SerialNumber,
                CheckCertificateDate = config.EthalonPressure.Category,
                CheckCertificateNumber = config.EthalonPressure.RegNum,
            });
            ethalons.Add(new EthalonDto()
            {
                Type = config.EthalonVoltage.SensorType,
                Title = config.EthalonVoltage.Title,
                RangeClass = config.EthalonVoltage.ErrorClass,
                SerialNumber = config.EthalonVoltage.SerialNumber,
                CheckCertificateDate = config.EthalonVoltage.Category,
                CheckCertificateNumber = config.EthalonVoltage.RegNum,
            });
        }

        private static void ApplyVariation(PressureSensorResultVM result, PressureSensorReportDto reportMain)
        {
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
                if (point.Result == null)
                {
                    varAcPoint.dU = "";
                    varAcPoint.Uf = "";
                    varAcPoint.Ur = "";
                }
                else
                {
                    varAcPoint.dU = point.Result.Uvar.ToString("F3");
                    varAcPoint.Uf = point.Result.UReal.ToString("F3");
                    varAcPoint.Ur = point.Result.Uback.ToString("F3");
                }
            }
            reportMain.VariationAccurancy = varAccur.OrderBy(el => int.Parse(el.PressurePoint.ToString())).ToArray();
        }

        private static void ApplyMainAccurancy(PressureSensorResultVM result, PressureSensorReportDto reportMain)
        {
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
                if (point.Result == null)
                {
                    mainAcPoint.U = "";
                    mainAcPoint.dU = "";
                }
                else
                {
                    mainAcPoint.U = point.Result.UReal.ToString("F3");
                    mainAcPoint.dU = point.Result.dUReal.ToString("F3");
                }
            }
            reportMain.MainAccurancy = mainAccur.OrderBy(el => int.Parse(el.PressurePoint.ToString())).ToArray();
        }

        private static void ApplyCommonData(PressureSensorCheckConfigVm config, PressureSensorResultVM result, PressureSensorReportDto reportMain)
        {
            reportMain.User = config.User;
            reportMain.CertificateNumber = config.SertificateNumber;
            reportMain.CertificateDate = config.SertificateDate;
            reportMain.Assay = result.Assay;
            reportMain.CommonResult = result.CommonResult;
            if (result.TimeStamp == null)
            {
                reportMain.CertificateDate = "";
                reportMain.ReportTime = "";
            }
            else
            {
                reportMain.CertificateDate = result.TimeStamp.Value.ToString("dd.MM.yy");
                reportMain.ReportTime = result.TimeStamp.Value.ToString("dd.MM.yy");
            }
            reportMain.ReportNumber = config.RegNum;
            reportMain.TypeDevice = config.SensorType;
            reportMain.SerialNumber = config.SerialNumber;
            reportMain.Owner = config.Master;
            reportMain.Temperature = config.Temperature.ToString("F0");
            reportMain.Humidity = config.Humidity.ToString("F0");
            reportMain.Pressure = config.DayPressure.ToString("F0");
            reportMain.Voltage = config.CommonVoltage.ToString("F0");
            reportMain.VisualCheckResult = result.VisualCheckResult;
            reportMain.LeakCheckResult = result.Leak;
            reportMain.CommonResult = result.CommonResult;
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