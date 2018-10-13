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

namespace PACESeriesUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow //: Window
    {
        private readonly PacePresenter _peresenter;

        public MainWindow()
        {
            InitializeComponent();
            ViewViewmodelMatcher.AddMatch(this.Resources, ViewAttribute.CheckView, ViewAttribute.CheckViewModelCashOnly);
            var vm = new PaceViewModel();
            _peresenter = new PacePresenter(vm, vm);
            DataContext = vm;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _peresenter.Dispose();
        }
    }
}
