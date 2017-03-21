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
using KipTM.Manuals.ViewModel;
using Tools.View;

namespace KipTM.Manuals.View
{
    /// <summary>
    /// Логика взаимодействия для DocsView.xaml
    /// </summary>
    [View(typeof(DocsViewModel))]
    public partial class DocsView : UserControl
    {
        public DocsView()
        {
            InitializeComponent();
        }
    }
}
