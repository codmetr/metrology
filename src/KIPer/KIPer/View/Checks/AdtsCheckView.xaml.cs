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
using ADTSChecks.Checks.ViewModel;
using KipTM.ViewModel.Checks;
using Tools;

namespace KipTM.View.Checks
{
    /// <summary>
    /// Interaction logic for ADTSCalibrationView.xaml
    /// </summary>
    [View(typeof(CheckBaseViewModel))]
    public partial class AdtsCheckView : UserControl
    {
        public AdtsCheckView()
        {
            InitializeComponent();
        }
    }
}
