using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Interfaces;
using PressureSensorData;

namespace PressureSensorCheck.Report
{
    /// <summary>
    /// Обновление DTO для протокола поверки
    /// </summary>
    public class ReportUpdater
    {
        /// <summary>
        /// Обновить протокол поверки
        /// </summary>
        /// <param name="id"></param>
        /// <param name="config">конфигурация</param>
        /// <param name="result">результат</param>
        /// <param name="report">отчет</param>
        public void Update(TestResultID id, PressureSensorConfig config, PressureSensorResult result, PressureSensorReportDto report)
        {
            ApplyCommonData(id, config, result, report);

            ApplyEtalons(config, report);

            // Заполнение результатов проверки основной погрешности
            ApplyMainAccurancy(result, report);

            // Заполнение результатов проверки вариации
            ApplyVariation(result, report);
        }

        private static void ApplyCommonData(TestResultID id, PressureSensorConfig config, PressureSensorResult result, PressureSensorReportDto report)
        {
            report.User = config.User;
            report.CertificateNumber = config.CertificateNumber;
            report.CertificateDate = config.CertificateDate;
            report.Assay = result.Assay;
            report.ReportNumber = config.ReportNumber;
            report.ReportTime = config.ReportDate;
            report.TypeDevice = config.SensorType;

            report.InUnit = config.Unit.ToStringLocalized(CultureInfo.CurrentUICulture);
            report.OutUnit = Units.mA.ToStringLocalized(CultureInfo.CurrentUICulture); //TODO: конфигурировать

            report.SerialNumber = id.SerialNumber;
            report.Owner = config.Master;
            report.Temperature = config.Temperature.ToString("F0");
            report.Humidity = config.Humidity.ToString("F0");
            report.Pressure = config.DayPressure.ToString("F0");
            report.Voltage = config.CommonVoltage.ToString("F0");
            report.CheckLawBase = config.ChecklLawBase;
            report.VisualCheckResult = result.VisualCheckResult;
            report.LeakCheckResult = result.Leak;
            report.CommonResult = result.CommonResult;
        }

        private void ApplyEtalons(PressureSensorConfig config, PressureSensorReportDto report)
        {
            var etalons = new List<EtalonDto>();
            etalons.Add(new EtalonDto()
            {
                Type = config.EtalonPressure.SensorType,
                Title = config.EtalonPressure.Title,
                RangeClass = config.EtalonPressure.ErrorClass,
                SerialNumber = config.EtalonPressure.SerialNumber,
                CheckCertificateDate = config.EtalonPressure.Category,
                CheckCertificateNumber = config.EtalonPressure.RegNum,
            });
            etalons.Add(new EtalonDto()
            {
                Type = config.EtalonOut.SensorType,
                Title = config.EtalonOut.Title,
                RangeClass = config.EtalonOut.ErrorClass,
                SerialNumber = config.EtalonOut.SerialNumber,
                CheckCertificateDate = config.EtalonOut.Category,
                CheckCertificateNumber = config.EtalonOut.RegNum,
            });
            report.Etalons = etalons.ToArray();
        }

        private static void ApplyMainAccurancy(PressureSensorResult result, PressureSensorReportDto report)
        {
            var mainAccur = (report.MainAccurancy ?? new List<MainAccurancyPointDto>()).ToList();
            foreach (var point in result.Points)
            {
                var mainAcPoint = mainAccur.FirstOrDefault(
                        el => el.Index.ToString() == point.Index.ToString());
                if (mainAcPoint == null)
                {
                    mainAcPoint = new MainAccurancyPointDto()
                    {
                        Index = point.Index.ToString(),
                        PressurePoint = point.Config.PressurePoint.ToString("F0"),
                        Uet = point.Config.OutPoint.ToString("F3"),
                        dUet = point.Config.Tollerance.ToString("F3"),
                    };
                    mainAccur.Add(mainAcPoint);
                }
                var resPoint = point.Result;
                if (resPoint == null)
                {
                    mainAcPoint.U = "";
                    mainAcPoint.dU = "";
                }
                else
                {
                    var dU = resPoint.OutPutValue - resPoint.VoltagePoint;
                    mainAcPoint.U = resPoint.OutPutValue.ToString("F3");
                    mainAcPoint.dU = dU.ToString("F3");
                }
            }
            report.MainAccurancy = mainAccur.OrderBy(el => int.Parse(el.Index.ToString())).ToArray();
        }

        private static void ApplyVariation(PressureSensorResult result, PressureSensorReportDto report)
        {
            var varAccur = (report.VariationAccurancy ?? new List<VariationAccurancyPointDto>()).ToList();
            foreach (var point in result.Points)
            {
                var varAcPoint = varAccur.FirstOrDefault(
                    el => el.Index.ToString() == point.Index.ToString());
                if (varAcPoint == null)
                {
                    varAcPoint = new VariationAccurancyPointDto()
                    {
                        Index = point.Index.ToString(),
                        PressurePoint = point.Config.PressurePoint.ToString("F0"),
                        dUet = point.Config.Tollerance.ToString("F3"),
                    };
                    varAccur.Add(varAcPoint);
                }
                var resPoint = point.Result;
                if (resPoint == null)
                {
                    varAcPoint.dU = "";
                    varAcPoint.Uf = "";
                    varAcPoint.Ur = "";
                }
                else
                {
                    var variation = Math.Abs(resPoint.OutPutValue - resPoint.OutPutValueBack);
                    varAcPoint.dU = variation.ToString("F3");
                    varAcPoint.Uf = resPoint.OutPutValue.ToString("F3");
                    varAcPoint.Ur = resPoint.OutPutValueBack.ToString("F3");
                }
            }
            report.VariationAccurancy = varAccur.OrderBy(el => int.Parse(el.PressurePoint.ToString())).ToArray();
        }
    }
}
