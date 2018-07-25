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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Manometer : UserControl
    {
        #region Dependency propertyes
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(Manometer), new PropertyMetadata(0.0, ValuePropertyChanged));

        private static void ValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            Manometer instance = dependencyObject as Manometer;

            if (instance == null)
            {
                return;
            }
            var anglePerValue = (instance.MaxAngle - instance.MinAngle) / (instance.MaxValue - instance.MinValue);
            instance.AngleValue = instance.MinAngle + (instance.Value * anglePerValue);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty AngleValueProperty = DependencyProperty.Register(
                   "AngleValue", typeof(double), typeof(Manometer), new PropertyMetadata(-120.0));

        public double AngleValue
        {
            get { return (double)GetValue(AngleValueProperty); }
            set { SetValue(AngleValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(double), typeof(Manometer), new PropertyMetadata(0.0));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(double), typeof(Manometer), new PropertyMetadata(100.0));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty ScaleStepProperty = DependencyProperty.Register(
            "ScaleStep", typeof(double), typeof(Manometer), new PropertyMetadata(10.0));

        public double ScaleStep
        {
            get { return (double)GetValue(ScaleStepProperty); }
            set { SetValue(ScaleStepProperty, value); }
        }

        public static readonly DependencyProperty SubscaleStepProperty = DependencyProperty.Register(
            "SubscaleStep", typeof(double), typeof(Manometer), new PropertyMetadata(5.0));

        public double SubscaleStep
        {
            get { return (double)GetValue(SubscaleStepProperty); }
            set { SetValue(SubscaleStepProperty, value); }
        }

        public static readonly DependencyProperty MinAngleProperty = DependencyProperty.Register(
            "MinAngle", typeof(double), typeof(Manometer), new PropertyMetadata(-120.0));

        public double MinAngle
        {
            get { return (double)GetValue(MinAngleProperty); }
            set { SetValue(MinAngleProperty, value); }
        }

        public static readonly DependencyProperty MaxAngleProperty = DependencyProperty.Register(
            "MaxAngle", typeof(double), typeof(Manometer), new PropertyMetadata(120.0));

        public double MaxAngle
        {
            get { return (double)GetValue(MaxAngleProperty); }
            set { SetValue(MaxAngleProperty, value); }
        }

        public static readonly DependencyProperty ValueTextFormatProperty = DependencyProperty.Register(
            "ValueTextFormat", typeof(string), typeof(Manometer), new PropertyMetadata("F0"));

        public string ValueTextFormat
        {
            get { return (string)GetValue(ValueTextFormatProperty); }
            set { SetValue(ValueTextFormatProperty, value); }
        }
        #endregion

        public Manometer()
        {
            InitializeComponent();
        }
    }
}
