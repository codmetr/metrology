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
using ADTSData;
using Tools;

namespace KipTM.View.Checks.Steps
{
    /// <summary>
    /// Interaction logic for AdtsPointResultView.xaml
    /// </summary>
    [View(typeof(AdtsPointResult))]
    public partial class AdtsPointResultView : UserControl
    {
        public AdtsPointResultView()
        {
            InitializeComponent();
        }
    }
}
