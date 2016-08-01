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
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var report = new ADTSReporter();
            var commonData = new AdtsCommonReportData()
            {
                ReportNumber = "123",
                DeviceType = "qqq"
            };
            IEnumerable<AdtsReportData> reportData = new List<AdtsReportData>()
            {
                new AdtsReportData(){Point = "p01", RealValue = "r02", Tolerance = "t03", ErrorValue = "e04", IsCorrect = "itrue"},
                new AdtsReportData(){Point = "p11", RealValue = "r12", Tolerance = "t13", ErrorValue = "e14", IsCorrect = "itrue"},
                new AdtsReportData(){Point = "p01", RealValue = "r02", Tolerance = "t03", ErrorValue = "e04", IsCorrect = "itrue"},
                new AdtsReportData(){Point = "p11", RealValue = "r12", Tolerance = "t13", ErrorValue = "e14", IsCorrect = "itrue"}
            };
            crystalReportViewer1.ReportSource = report.GetReport(commonData, reportData);
        }
    }
}
