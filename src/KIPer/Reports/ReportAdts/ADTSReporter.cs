using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using KipTM.Model.Checks;
using ReportService;
using ArchiveData.DTO;

namespace ReportAdts
{
    [Report(typeof(ADTSTestMethod))]
    public class ADTSReporter: IReporter
    {
        public ReportClass GetReport(AdtsCommonReportData commonData, IEnumerable<AdtsReportData> staticData, IEnumerable<AdtsReportData> dinamicData)
        {
            var result = new ADTSCheckReport();
            result.SetDataSource(new []{commonData});
            result.Subreports["staticChannel"].SetDataSource(staticData);
            result.Subreports["dinamicChannel"].SetDataSource(dinamicData);
            return result;
        }

        public object GetReport(TestResult result)
        {
            var ethalon = result.Etalon.FirstOrDefault();
            var commonData = new AdtsCommonReportData()
            {
                CheckDate = result.Timestamp.ToString("d"),
                DeviceType = result.TargetDevice.DeviceType,
                CheckMethod = result.CheckType,
                SerialNumber = result.TargetDevice.SerialNumber,
                ChannelRange = result.Channel,
                AtmosphericPressure = result.AtmospherePressure,
                Temperature = result.Temperature,
            };
            if (ethalon != null)
            {
                commonData.EthalonDeviceType = ethalon.DeviceType;
            }

            var checkResults = new List<AdtsReportData>();
            var testResults = new List<AdtsReportData>();

            foreach (var stepResult in result.Results)
            {
                
            }

            return GetReport(commonData, checkResults, testResults);
        }
    }
}
