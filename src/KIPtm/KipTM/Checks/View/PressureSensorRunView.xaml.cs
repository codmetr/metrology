using System.Windows.Controls;
using PressureSensorCheck.Workflow;
using Tools.View;

namespace KipTM.Checks.View
{
    /// <summary>
    /// Interaction logic for PressureSensorRunView.xaml
    /// </summary>
    [View(typeof(PressureSensorRunVm))]
    public partial class PressureSensorRunView : UserControl
    {
        public PressureSensorRunView()
        {
            InitializeComponent();
        }
    }
}
