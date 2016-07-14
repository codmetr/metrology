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
using KipTM.ViewModel.Checks;

namespace KipTM.View.Checks
{
    /// <summary>
    /// Interaction logic for MechanicalManometerView.xaml
    /// </summary>
    [ViewAttribute(typeof(MechanicalManometerViewModel))]
    public partial class MechanicalManometerView : UserControl
    {
        public MechanicalManometerView()
        {
            InitializeComponent();
        }
    }
}
