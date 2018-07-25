using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CrystalDecisions.CrystalReports.Engine;
using KipTM.ViewModel.Report;
using Tools;
using Tools.View;
using UserControl = System.Windows.Controls.UserControl;

namespace KipTM.View
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    [View(typeof(ReportViewModel))]
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
        }

        private void WindowsFormsHost_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Report.SetSource(Host.DataContext as ReportClass);
        }
    }
}
