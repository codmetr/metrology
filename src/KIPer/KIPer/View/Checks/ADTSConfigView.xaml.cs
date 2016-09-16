using System.Windows.Controls;
using ADTSChecks.ViewModel.Checks;
using KipTM.ViewModel.Checks;
using Tools;

namespace KipTM.View.Checks
{
    /// <summary>
    /// Interaction logic for ADTSConfigView.xaml
    /// </summary>
    [View(typeof(ADTSCheckConfigViewModel))]
    public partial class ADTSConfigView : UserControl
    {
        public ADTSConfigView()
        {
            InitializeComponent();
        }
    }
}
