using System.Windows.Controls;
using Tools.View;
using VisaChannel.TransportChannels.Visa;

namespace VisaChannelView
{
    /// <summary>
    /// Interaction logic for VisaSettingsView.xaml
    /// </summary>
    [View(typeof(VisaSettings))]
    public partial class VisaSettingsView : UserControl
    {
        public VisaSettingsView()
        {
            InitializeComponent();
        }
    }
}
