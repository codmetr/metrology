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
    /// Interaction logic for ErrorLine.xaml
    /// </summary>
    public partial class ErrorLine : UserControl
    {
        public static readonly DependencyProperty StringValueProperty = DependencyProperty.Register(
            "StringValue", typeof (string), typeof (ErrorLine), new PropertyMetadata("123"));

        public string StringValue
        {
            get { return (string) GetValue(StringValueProperty); }
            set { SetValue(StringValueProperty, value); }
        }

        public static readonly DependencyProperty IncrementCommandProperty = DependencyProperty.Register(
            "IncrementCommand", typeof (ICommand), typeof (ErrorLine), new PropertyMetadata(default(ICommand)));

        public ICommand IncrementCommand
        {
            get { return (ICommand) GetValue(IncrementCommandProperty); }
            set { SetValue(IncrementCommandProperty, value); }
        }

        public static readonly DependencyProperty DecrementCommandProperty = DependencyProperty.Register(
            "DecrementCommand", typeof (ICommand), typeof (ErrorLine), new PropertyMetadata(default(ICommand)));

        public ICommand DecrementCommand
        {
            get { return (ICommand) GetValue(DecrementCommandProperty); }
            set { SetValue(DecrementCommandProperty, value); }
        }

        public ErrorLine()
        {
            InitializeComponent();
        }
    }
}
