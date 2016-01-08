using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Scale.xaml
    /// </summary>
    public partial class Scale : Shape
    {
        #region Dependency propertyes
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(Scale), new PropertyMetadata(45.0));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            "StartAngle", typeof(double), typeof(Scale), new PropertyMetadata(-90.0));

        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly DependencyProperty StopAngleProperty = DependencyProperty.Register(
            "StopAngle", typeof(double), typeof(Scale), new PropertyMetadata(90.0));

        public double StopAngle
        {
            get { return (double)GetValue(StopAngleProperty); }
            set { SetValue(StopAngleProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(double), typeof(Scale), new PropertyMetadata(0.0));

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(double), typeof(Scale), new PropertyMetadata(10.0));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(double), typeof(Scale), new PropertyMetadata(1.0));

        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public static readonly DependencyProperty SubStepProperty = DependencyProperty.Register(
            "SubStep", typeof(double), typeof(Scale), new PropertyMetadata(0.5));

        public double SubStep
        {
            get { return (double)GetValue(SubStepProperty); }
            set { SetValue(SubStepProperty, value); }
        }

        public static readonly DependencyProperty StepFillProperty = DependencyProperty.Register(
            "StepFill", typeof(Brush), typeof(Scale), new PropertyMetadata(default(Brush)));

        public Brush StepFill
        {
            get { return (Brush)GetValue(StepFillProperty); }
            set { SetValue(StepFillProperty, value); }
        }

        public static readonly DependencyProperty SubStepFillProperty = DependencyProperty.Register(
            "SubStepFill", typeof(Brush), typeof(Scale), new PropertyMetadata(default(Brush)));

        public Brush SubStepFill
        {
            get { return (Brush)GetValue(SubStepFillProperty); }
            set { SetValue(SubStepFillProperty, value); }
        }

        public static readonly DependencyProperty ValueTextFormatProperty = DependencyProperty.Register(
            "ValueTextFormat", typeof (string), typeof (Scale), new PropertyMetadata("F2"));

        public string ValueTextFormat
        {
            get { return (string) GetValue(ValueTextFormatProperty); }
            set { SetValue(ValueTextFormatProperty, value); }
        }

        #endregion

        public Scale()
        {
            InitializeComponent();
        }

        GeometryGroup _g = new GeometryGroup();

        protected override Geometry DefiningGeometry
        {
            get
            {
                Redraw(_g);
                return _g;
            }
        }

        private void Redraw(GeometryGroup g)
        {
            if (g == null)
                return;
            _g.Children.Clear();
            _g = DrawScale(_g);
        }

        private GeometryGroup DrawScale(GeometryGroup g)
        {
            if (StartAngle > StopAngle)
            {
                return g;
            }

            if (MinValue > MaxValue)
            {
                return g;
            }

            if (Step <= 0)
            {
                return g;
            }

            if (SubStep <= 0)
            {
                return g;
            }

            var startAngle = StartAngle / 180 * Math.PI;
            var endAngle = StopAngle / 180 * Math.PI;

            var xCenter = this.Radius;
            var yCenter = Radius;

            var heigthScale = Radius / 5;
            var widthScale = heigthScale / 5;

            var heigthSubScale = heigthScale / 2;
            var widthSubScale = heigthSubScale / 5;
            g.FillRule = FillRule.Nonzero;
            DrawScale(g, xCenter, yCenter, startAngle, endAngle, heigthScale, widthScale, MinValue, MaxValue, Step,
                Radius, true);

            DrawScale(g, xCenter, yCenter, startAngle, endAngle, heigthSubScale, widthSubScale, MinValue, MaxValue, SubStep,
                Radius);

            return g;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="xCenter"></param>
        /// <param name="yCenter"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="heigthScale"></param>
        /// <param name="widthScale"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="stepValue"></param>
        /// <param name="radius"></param>
        /// <remarks>
        /// calc method:
        /// 0 |______________________________y
        ///   |
        ///   |
        ///   |
        ///   |     5 
        ///   |      /2*6
        ///   |     / / /
        ///   |    / / /
        ///   |   / / /
        ///   | 3/ / /
        ///   |  1*4/
        ///   |  /
        ///   |a/
        ///  x|/__________________________
        ///  
        ///   1 - startPoint
        ///   2 - endPoint
        ///   3 - lstartPoint
        ///   4 - rstartPoint
        ///   5 - lendPoint
        ///   6 - rendPoint
        /// </remarks>
        private void DrawScale(GeometryGroup g,
            double xCenter, double yCenter, double startAngle, double endAngle,
            double heigthScale, double widthScale,
            double startValue, double endValue, double stepValue, double radius, bool withText = false)
        {
            var anglePerValue = (endAngle - startAngle) / (MaxValue - MinValue);
            for (var value = startValue; value <= endValue; value += stepValue)
            {
                var angle = startAngle + (value - startValue) * anglePerValue;

                var xEndPoint = xCenter + (radius * Math.Sin(angle));
                var yEndPoint = yCenter - (radius * Math.Cos(angle));
                var xlEndPoint = xEndPoint - (widthScale / 2) * Math.Cos(angle);
                var ylEndPoint = yEndPoint - (widthScale / 2) * Math.Sin(angle);
                var xrEndPoint = xEndPoint + (widthScale / 2) * Math.Cos(angle);
                var yrEndPoint = yEndPoint + (widthScale / 2) * Math.Sin(angle);

                var xStartPoint = xCenter + ((radius - heigthScale) * Math.Sin(angle));
                var yStartPoint = yCenter - ((radius - heigthScale) * Math.Cos(angle));
                var xlStartPoint = xStartPoint - (widthScale / 2) * Math.Cos(angle);
                var ylStartPoint = yStartPoint - (widthScale / 2) * Math.Sin(angle);
                var xrStartPoint = xStartPoint + (widthScale / 2) * Math.Cos(angle);
                var yrStartPoint = yStartPoint + (widthScale / 2) * Math.Sin(angle);

                var rectScale = new PathGeometry();
                rectScale.Figures.Add(new PathFigure(new Point(xlStartPoint, ylStartPoint), new List<PathSegment>
                    {
                        new LineSegment(new Point(xlEndPoint, ylEndPoint), false),
                        new LineSegment(new Point(xrEndPoint, yrEndPoint), false),
                        new LineSegment(new Point(xrStartPoint, yrStartPoint), false),
                    }, true));

                g.Children.Add(rectScale);
                if(!withText)
                    continue;
                FormattedText ft = new FormattedText(
                value.ToString(ValueTextFormat),
                Thread.CurrentThread.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface("Arial"), 8, Fill);
                var textHeight = ft.Height;
                var textWidth = ft.Width;
                var textDiagonale = Math.Sqrt(Math.Pow(textHeight, 2) + Math.Pow(textWidth, 2));
                var xtextPoint = xCenter + ((radius - heigthScale - (textDiagonale / 2)) * Math.Sin(angle)) - (textWidth/2);
                var ytextPoint = yCenter - ((radius - heigthScale - (textDiagonale / 2)) * Math.Cos(angle)) - (textHeight/2);
                g.Children.Add(ft.BuildGeometry(new Point(xtextPoint, ytextPoint)));

            }
        }
    }
}
