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

namespace PACETool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Pace5000Model _model;

        public MainWindow()
        {
            InitializeComponent();
            var vm = new Pace5000ConnectionVm(Dispatcher);
            DataContext = vm;
            _model = new Pace5000Model(vm.Pace, vm.Connection);
        }
    }
}
