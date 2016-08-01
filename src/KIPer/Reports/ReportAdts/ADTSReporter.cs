using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using ReportService;

namespace ReportAdts
{
    //[Report()]
    public class ADTSReporter
    {
        public ReportClass GetReport(AdtsCommonReportData commonData, IEnumerable<AdtsReportData> reportData)
        {
            var result = new ADTSCheckReport();
            result.SetDataSource(new []{commonData});
            var adtsReportDatas = reportData as AdtsReportData[] ?? reportData.ToArray();
            result.Subreports["staticChannel"].SetDataSource(adtsReportDatas);
            return result;
        }
    }
}
