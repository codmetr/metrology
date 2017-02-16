using System.Windows.Controls;
using PACEChecks.Channels.ViewModel;
using Tools.View;

namespace PACEView
{
    /// <summary>
    /// Interaction logic for PACE1000View.xaml
    /// </summary>
    [View(typeof(PaceEthalonChannelViewModel))]
    public partial class PACE1000EthalonChannelView : UserControl
    {
        public PACE1000EthalonChannelView()
        {
            InitializeComponent();
        }
    }
}
