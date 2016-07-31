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
            var result = new ADTSReport();
            //result.Load(@"\ADTSReporter.rpt");
            result.SetDataSource(new []{commonData});
//            result.SetDataSource(commonData);
            result.Subreports[@"StaticChannel"].SetDataSource(reportData.ToArray());
            result.Subreports[@"DinamicCannel"].SetDataSource(reportData.ToArray());
            return result;
        }
    }
}
