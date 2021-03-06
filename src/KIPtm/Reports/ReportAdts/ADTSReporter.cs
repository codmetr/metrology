﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ADTSData;
using CrystalDecisions.CrystalReports.Engine;
using ReportService;
using ArchiveData.DTO;
using CheckFrame.Checks;

namespace ReportAdts
{
    //[Report(typeof(ADTSTestMethod))]
    [Report(KeysDic.Test)]
    public class AdtsReporter: IReporter
    {
        public ReportClass GetReport(AdtsCommonReportData commonData, IEnumerable<AdtsReportData> staticData, IEnumerable<AdtsReportData> dinamicData)
        {
            var result = new ADTSCheckReport();
            result.SetDataSource(new []{commonData});
            result.Subreports["staticChannel"].SetDataSource(staticData);
            result.Subreports["dinamicChannel"].SetDataSource(dinamicData);
            return result;
        }

        public object GetReport(TestResultID resultId, CheckConfigData conf, object result)
        {
            var etalon = conf.Etalons.FirstOrDefault().Value;
            var commonData = new AdtsCommonReportData()
            {
                CheckDate = resultId.Timestamp.ToString("d"),
                DeviceType = conf.TargetDevice.Device.DeviceType.Model,
                ChannelRange = getRangeFromType(conf.TargetDevice.Device.DeviceType),
                CheckMethod = "K199",//result.CheckType,
                SerialNumber = conf.TargetDevice.Device.SerialNumber,
                AtmosphericPressure = conf.AtmospherePressure,
                Temperature = conf.Temperature,
                ReportNumber = "123"
            };
            if (etalon != null)
            {
                commonData.EtalonDeviceType = etalon.Device.DeviceType;
                commonData.EtalonError = getErrorFromType(etalon.Device.DeviceType);
                commonData.EtalonChannelRange = getRangeFromType(etalon.Device.DeviceType);
            }

            var staticResults = new List<AdtsReportData>();
            var dinamicResults = new List<AdtsReportData>();

            var resPoints = result as IEnumerable<TestStepResult>;
            foreach (var stepResult in resPoints.Where(res => res.ChannelKey == KeysDic.KeySettingsPS))
            {
                var res = stepResult.Result as AdtsPointResult;
                if (res==null)
                    continue;
                staticResults.Add(dataToReport(res));
            }
            
            foreach (var stepResult in resPoints.Where(res => res.ChannelKey == KeysDic.KeySettingsPT))
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
            return type.Model == KeysDic.ADTSModelKey
                ? "Ps (35…1128) mbar;\nPt  (35…3500) mbar"
                : type.Model == KeysDic.PACE1000ModelKey ? "1300mbar, 3500 mbar" : "";
        }

        private string getErrorFromType(DeviceTypeDescriptor type)
        {
            return type.Model == KeysDic.PACE1000ModelKey ? "± 0,005% ВПИ" : "";
        }
    }
}
