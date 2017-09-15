using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KipTM.Report.PressureSensor;
using Tools.View;

namespace KipTM.Checks.View
{
    /// <summary>
    /// Interaction logic for PressureSensorReportView.xaml
    /// </summary>
    [View(typeof(PressureSensorReportViewModel))]
    public partial class PressureSensorReportView : UserControl
    {
        private bool _isMainReportLoaded = false;
        private bool _isCertificateReportLoaded = false;
        public PressureSensorReportView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MainReport_Loaded(object sender, RoutedEventArgs e)
        {
            if(_isMainReportLoaded)
                return;
            _isMainReportLoaded = true;
            var vm = DataContext as PressureSensorReportViewModel;
            if(vm == null)
                return;
            MainReport.ViewerCore.ReportSource = vm.MainReportData;
        }

        private void CertificateReport_Loaded(object sender, MouseButtonEventArgs e)
        {
            if(_isCertificateReportLoaded)
                return;
            _isCertificateReportLoaded = true;
            var vm = DataContext as PressureSensorReportViewModel;
            if(vm == null)
                return;
            CertificateReport.ViewerCore.ReportSource = vm.CertificateReportDate;

        }
    }
}
