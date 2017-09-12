using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            MainReportData.Load();
            //MainReportData.SetDataSource(new[] { mainReportData});
            CertificateReportDate = new PressureSensorСertificate();
            CertificateReportDate.Load();
            //CertificateReportDate.SetDataSource(new[] { certificateDto });
            //MainReportData = null;
            //CertificateReportDate = null;
        }

        /// <summary>
        /// Основной отчет
        /// </summary>
        public PressureSensorReport MainReportData { get; private set; }

        /// <summary>
        /// Отчет свидельство
        /// </summary>
        public PressureSensorСertificate CertificateReportDate { get; private set; }
    }
}
