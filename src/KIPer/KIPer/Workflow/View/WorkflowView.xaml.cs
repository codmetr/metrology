using System.Windows.Controls;
using KipTM.Workflow;
using Tools.View;

namespace KipTM.View.Checks
{
    /// <summary>
    /// Interaction logic for WorkflowView.xaml
    /// </summary>
    [View(typeof(LineWorkflow))]
    public partial class WorkflowView : UserControl
    {
        public WorkflowView()
        {
            InitializeComponent();
        }
    }
}
