using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using KipTM.Checks.ViewModel.Config;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using PressureSensorCheck.Workflow;
using Tools.View;

namespace KipTM.Checks.View
{
    /// <summary>
    /// Interaction logic for PressureSensorRunView.xaml
    /// </summary>
    [View(typeof(PressureSensorRunVm1))]
    public partial class PressureSensorRunView : UserControl
    {
        public PressureSensorRunView()
        {
            InitializeComponent();
        }
    }
}
