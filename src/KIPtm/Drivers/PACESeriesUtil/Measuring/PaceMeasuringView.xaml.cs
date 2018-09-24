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
using PACESeriesUtil.VM;
using Tools.View;

namespace PACESeriesUtil.Viewes
{
    /// <summary>
    /// Interaction logic for PaceMeasuringView.xaml
    /// </summary>
    [View(typeof(PaceMeasuringViewModel))]
    public partial class PaceMeasuringView : UserControl
    {
        public PaceMeasuringView()
        {
            InitializeComponent();
        }
    }
}
