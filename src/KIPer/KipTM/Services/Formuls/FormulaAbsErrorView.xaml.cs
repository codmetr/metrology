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
using KipTM.Services.ViewModel.FillReport;
using Tools.View;

namespace KipTM.Services.Formuls
{
    /// <summary>
    /// Interaction logic for FormulaAbsErrorView.xaml
    /// </summary>
    [View(typeof(FormulaAbsError))]
    public partial class FormulaAbsErrorView : UserControl
    {
        public FormulaAbsErrorView()
        {
            InitializeComponent();
        }
    }
}
