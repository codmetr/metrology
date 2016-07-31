using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ReportAdts;

namespace TestReport
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            var report = new ADTSReporter();
            var commonData = new AdtsCommonReportData();
            IEnumerable<AdtsReportData> reportData = new List<AdtsReportData>()
            {
                new AdtsReportData(){Point = "01", RealValue = "02", Tolerance = "03", ErrorValue = "04", IsCorrect = "true"},
                new AdtsReportData(){Point = "11", RealValue = "12", Tolerance = "13", ErrorValue = "14", IsCorrect = "true"}
            };
            InitializeComponent();
            crystalReportViewer1.ReportSource = report.GetReport(commonData, reportData);
        }
    }
}
