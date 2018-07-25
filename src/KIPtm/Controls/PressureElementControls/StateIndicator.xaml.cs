using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PressureElementControls
{
    /// <summary>
    /// Interaction logic for StateIndicator.xaml
    /// </summary>
    public partial class StateIndicator : UserControl
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof (int), typeof (StateIndicator), new PropertyMetadata(0));

        public int State
        {
            get
            {
                return (int) GetValue(StateProperty);
            }
            set
            {
                SetValue(StateProperty, value);
            }
        }

        public StateIndicator()
        {
            InitializeComponent();
        }
    }
}
