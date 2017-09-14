using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrystalDecisions.CrystalReports.Engine;

namespace KipTM.Report.PressureSensor
{
    /// <summary>
    /// Визуальная модель отчетов
    /// </summary>
    public class PressureSensorReportViewModel
    {
        /// <summary>
        /// Визуальная модель отчетов
        /// </summary>
        /// <param name="mainReportData"></param>
        /// <param name="certificateDto"></param>
        public PressureSensorReportViewModel(PressureSensorReportDto mainReportData, PressureSensorCertificateDto certificateDto)
        {
            MainReportData = new PressureSensorReport();
            //MainReportData = new BlankReport();
            MainReportData.Load();
            MainReportData.SetDataSource(new[] { mainReportData});
            //CertificateReportDate = new PressureSensorСertificate();
            CertificateReportDate = new BlankReport();
            CertificateReportDate.Load();
            //CertificateReportDate.SetDataSource(new[] { certificateDto });
            //MainReportData = null;
            //CertificateReportDate = null;
        }

        /// <summary>
        /// Основной отчет
        /// </summary>
        public ReportClass MainReportData { get; private set; }

        /// <summary>
        /// Отчет свидельство
        /// </summary>
        public ReportClass CertificateReportDate { get; private set; }
    }
}
