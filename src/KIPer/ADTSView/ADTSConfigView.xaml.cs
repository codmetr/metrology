using System.Windows.Controls;
using ADTSChecks.Checks.ViewModel;
using Tools;
using Tools.View;

namespace KipTM.View.Checks
{
    /// <summary>
    /// Interaction logic for ADTSConfigView.xaml
    /// </summary>
    [View(typeof(CheckConfigViewModel))]
    public partial class ADTSConfigView : UserControl
    {
        public ADTSConfigView()
        {
            InitializeComponent();
        }
    }
}
