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
using ArchiveData.DTO;

namespace KipTM.Checks.View
{
    /// <summary>
    /// Interaction logic for ChannelDescriptorView.xaml
    /// </summary>
    [Tools.View.View(typeof(ChannelDescriptor))]
    public partial class ChannelDescriptorView : UserControl
    {
        public ChannelDescriptorView()
        {
            InitializeComponent();
        }
    }
}
