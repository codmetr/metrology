using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Devices;
using ADTSData;
using CrystalDecisions.CrystalReports.Engine;
using ReportService;
using ArchiveData.DTO;

namespace ReportAdts
{
    //[Report(typeof(ADTSTestMethod))]
    [Report(Test.Key)]
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
                DeviceType = result.TargetDevice.DeviceType.Model,
                ChannelRange = getRangeFromType(result.TargetDevice.DeviceType),
                CheckMethod = "K199",//result.CheckType,
                SerialNumber = result.TargetDevice.SerialNumber,
                AtmosphericPressure = result.AtmospherePressure,
                Temperature = result.Temperature,
                ReportNumber = "123"
            };
            if (ethalon != null)
            {
                commonData.EthalonDeviceType = ethalon.DeviceType;
                commonData.EthalonError = getErrorFromType(ethalon.DeviceType);
                commonData.EthalonChannelRange = getRangeFromType(ethalon.DeviceType);
            }

            var staticResults = new List<AdtsReportData>();
            var dinamicResults = new List<AdtsReportData>();


            foreach (var stepResult in result.Results.Where(res => res.ChannelKey == CheckBase.KeySettingsPS))
            {
                var res = stepResult.Result as AdtsPointResult;
                if (res==null)
                    continue;
                staticResults.Add(dataToReport(res));
            }

            foreach (var stepResult in result.Results.Where(res => res.ChannelKey == CheckBase.KeySettingsPT))
            {
                var res = stepResult.Result as AdtsPointResult;
                if (res==null)
                    continue;
                dinamicResults.Add(dataToReport(res));
            }

            return GetReport(commonData, staticResults, dinamicResults);
        }

        private AdtsReportData dataToReport(AdtsPointResult data)
        {
            return new AdtsReportData()
            {
                Point = data.Point.ToString("f2"),
                Tolerance = data.Tolerance.ToString("f2"),
                ErrorValue = data.Error.ToString("f2"),
                RealValue = data.RealValue.ToString("f2"),
                IsCorrect = data.IsCorrect ? "соответствует" : "не соответствует",
            };
        }

        private string getRangeFromType(DeviceTypeDescriptor type)
        {
            return type.Model == ADTSModel.Model
                ? "Ps (35…1128) mbar;\nPt  (35…3500) mbar"
                : type.Model == PACE1000Model.Model ? "1300mbar, 3500 mbar" : "";
        }

        private string getErrorFromType(DeviceTypeDescriptor type)
        {
            return type.Model == PACE1000Model.Model ? "± 0,005% ВПИ" : "";
        }
    }
}
