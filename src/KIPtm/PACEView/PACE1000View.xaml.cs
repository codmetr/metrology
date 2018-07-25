using System.Windows.Controls;
using PACEChecks.Services;
using Tools.View;

namespace PACEView
{
    /// <summary>
    /// Interaction logic for PACE5000View.xaml
    /// </summary>
    [View(typeof(Pace1000ViewModel))]
    public partial class PACE1000View : UserControl
    {


        public PACE1000View()
        {
            InitializeComponent();
        }
    }
}
