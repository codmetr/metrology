using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using KipTM.Report.PressureSensor;

namespace PressureSensorCheck.Report
{
    /// <summary>
    /// Визуальная модель отчетов
    /// </summary>
    public class PressureSensorReportViewModel
    {
        private readonly PressureSensorReportDto _mainReportDto;
        private readonly PressureSensorCertificateDto _certificateDto;

        private ReportClass _mainReportData;
        private ReportClass _certificateReportDate;

        /// <summary>
        /// Визуальная модель отчетов
        /// </summary>
        /// <param name="mainReportDto"></param>
        /// <param name="certificateDto"></param>
        public PressureSensorReportViewModel(PressureSensorReportDto mainReportDto, PressureSensorCertificateDto certificateDto)
        {
            _mainReportDto = mainReportDto;
            _certificateDto = certificateDto;
        }

        /// <summary>
        /// Основной отчет
        /// </summary>
        public ReportClass MainReportData
        {
            get
            {
                if (_mainReportData == null)
                {
                    _mainReportData = new PressureSensorReport();
                    //_mainReportData = new BlankReport();
                    _mainReportData.FileName = Path.Combine("Report",_mainReportData.ResourceName);
                    _mainReportData.Load();
                }
                _mainReportData.SetDataSource(new[] { _mainReportDto });
                _mainReportData.Subreports["EthalonRep"].SetDataSource(_mainReportDto.Ethalons);
                _mainReportData.Subreports["MainError"].SetDataSource(_mainReportDto.MainAccurancy);
                _mainReportData.Subreports["VariationReport"].SetDataSource(_mainReportDto.VariationAccurancy);
                return _mainReportData;
            }
        }

        /// <summary>
        /// Отчет свидельство
        /// </summary>
        public ReportClass CertificateReportDate
        {
            get
            {
                if (_certificateReportDate == null)
                {
                    _certificateReportDate = new PressureSensorСertificate();
                    _certificateReportDate.FileName = Path.Combine("Report", _certificateReportDate.ResourceName);
                    _certificateReportDate.Load();
                }
                _certificateReportDate.SetDataSource(new[] { _certificateDto });
                return _certificateReportDate;
            }
        }
    }
}
