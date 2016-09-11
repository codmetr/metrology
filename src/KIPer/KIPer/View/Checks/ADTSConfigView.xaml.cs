using System.Windows.Controls;
using KipTM.ViewModel.Checks;

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
