using System.Windows.Controls;
using CheckFrame.Workflow;
using Tools.View;

namespace KipTM.Workflow.View
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
