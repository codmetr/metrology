using System;
using System.Collections.Generic;
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
using PACEChecks.Channels.ViewModel;
using Tools.View;

namespace PACEView
{
    /// <summary>
    /// Interaction logic for PACEConfigView.xaml
    /// </summary>
    [View(typeof(PaceConfigViewModel))]
    public partial class PACEConfigView : UserControl
    {
        public PACEConfigView()
        {
            InitializeComponent();
        }
    }
}
