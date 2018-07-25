using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace KipTM.View
{
    public partial class ReportHost : UserControl
    {
        public ReportHost()
        {
            InitializeComponent();
        }

        public void SetSource(ReportClass source)
        {
            reportViewer.ReportSource = source;
        }
    }
}
