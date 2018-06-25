using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using PressureSensorData;
using ReportService;

namespace PressureSensorCheck.Report
{
    public class ReportFactory: IReportFactory
    {
        /// <summary>
        /// Получить отчет по типу проверки
        /// </summary>
        /// <param name="resultId">Идентификатор проверки</param>
        /// <param name="confObj">конфигурация проверки</param>
        /// <param name="resultObj">результат проверки</param>
        /// <returns>визуальная модель отчета(ов)</returns>
        public object GetReporter(TestResultID resultId, object confObj, object resultObj)
        {
            if (resultId == null || confObj == null || resultObj == null)
            {
                return null;
            }
            var conf = confObj as PressureSensorConfig;
            var result = resultObj as PressureSensorResult;

            if (conf == null || result == null)
                return null;// TODO Throw new Type result or config not expected 

            var reportUpdater = new ReportUpdater();
            var certificateUpdater = new CertificateUpdater();
            var reportMain = new PressureSensorReportDto();
            var reportCertificate = new PressureSensorCertificateDto();
            reportUpdater.Update(resultId, conf, result, reportMain);
            certificateUpdater.Update(resultId, conf, result, reportCertificate);

            var report = new PressureSensorReportViewModel(reportMain, reportCertificate);
            return report;
        }
    }
}
