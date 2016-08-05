using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ADTSData;
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
                CheckMethod = "K199",//result.CheckType,
                SerialNumber = result.TargetDevice.SerialNumber,
                ChannelRange = result.Channel,
                AtmosphericPressure = result.AtmospherePressure,
                Temperature = result.Temperature,
            };
            if (ethalon != null)
            {
                commonData.EthalonDeviceType = ethalon.DeviceType;
            }

            var staticResults = new List<AdtsReportData>();
            var dinamicResults = new List<AdtsReportData>();


            foreach (var stepResult in result.Results.Where(res => res.ChannelKey == ADTSMethodBase.KeySettingsPS))
            {
                var res = stepResult.Result as AdtsPointResult;
                if (res==null)
                    continue;

                staticResults.Add(new AdtsReportData()
                {
                    Point = res.Point.ToString("f2"),
                    Tolerance = res.Tolerance.ToString("f2"),
                    ErrorValue = res.Error.ToString("f2"),
                    RealValue = res.RealValue.ToString("f2"),
                    IsCorrect = res.IsCorrect ? "соответствует" : "не соответствует",
                });
            }

            foreach (var stepResult in result.Results.Where(res => res.ChannelKey == ADTSMethodBase.KeySettingsPT))
            {
                var res = stepResult.Result as AdtsPointResult;
                if (res==null)
                    continue;

                dinamicResults.Add(new AdtsReportData()
                {
                    Point = res.Point.ToString("f2"),
                    Tolerance = res.Tolerance.ToString("f2"),
                    ErrorValue = res.Error.ToString("f2"),
                    RealValue = res.RealValue.ToString("f2"),
                    IsCorrect = res.IsCorrect ? "соответствует" : "не соответствует",
                });
            }

            return GetReport(commonData, staticResults, dinamicResults);
        }
    }
}
