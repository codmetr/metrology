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
                ReportNumber = "1",
                DeviceType = "ADTS450",
                SerialNumber = "11111111",
                ChannelRange = "Ps(35...1128)\nmbar;\nPt(35...3500)\nmbar",
                CheckDate = "01.06.2016",
                EthalonDeviceType = "PACE 1000",
                EthalonChannelRange = "1300mbar, 3500mbar",
                EthalonError = "+/- 0,005% ВПИ",
                CheckMethod = "K199",
                AtmosphericPressure = "750,22 mmHg",
                Temperature = "21,0 C"
            };
            IEnumerable<AdtsReportData> staticData = new List<AdtsReportData>()
            {
                new AdtsReportData(){Point = "p01", RealValue = "r02", Tolerance = "t03", ErrorValue = "e04", IsCorrect = "соответствует"},
                new AdtsReportData(){Point = "p11", RealValue = "r12", Tolerance = "t13", ErrorValue = "e14", IsCorrect = "соответствует"},
                new AdtsReportData(){Point = "p01", RealValue = "r02", Tolerance = "t03", ErrorValue = "e04", IsCorrect = "соответствует"},
                new AdtsReportData(){Point = "p11", RealValue = "r12", Tolerance = "t13", ErrorValue = "e14", IsCorrect = "соответствует"}
            };
            IEnumerable<AdtsReportData> dinamicData = new List<AdtsReportData>()
            {
                new AdtsReportData(){Point = "p01", RealValue = "r02", Tolerance = "t03", ErrorValue = "e04", IsCorrect = "соответствует"},
                new AdtsReportData(){Point = "p11", RealValue = "r12", Tolerance = "t13", ErrorValue = "e14", IsCorrect = "соответствует"},
                new AdtsReportData(){Point = "p01", RealValue = "r02", Tolerance = "t03", ErrorValue = "e04", IsCorrect = "соответствует"},
                new AdtsReportData(){Point = "p11", RealValue = "r12", Tolerance = "t13", ErrorValue = "e14", IsCorrect = "соответствует"}
            };
            crystalReportViewer1.ReportSource = report.GetReport(commonData, staticData, dinamicData);
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
