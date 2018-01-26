using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using ZedGraph;
using Color = System.Drawing.Color;

namespace ZedgraphWpfSimple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource _cancellation = new CancellationTokenSource();
        private ZedGraphControl _graph;
        private GraphPane _pane;
        readonly PointPairList _points = new PointPairList();
        private LineItem _line;
        private PointPairList _pointsSin = new PointPairList();
        private LineItem _lineSin;
        private DateTime _lastXMax;
        private TimeSpan _windowWidth = TimeSpan.FromSeconds(15);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var cancel = _cancellation.Token;
            var signal = new Signal();
            var updater = new Updater();
            signal.Subscribe(updater);
            updater.NewMeasure += UpdaterOnNewMeasure;
            var signalSin = new SignalSin();
            var updaterSin = new Updater();
            signalSin.Subscribe(updaterSin);
            updaterSin.NewMeasure += UpdaterSinOnNewMeasure;
            _graph = GraphHost.Child as ZedGraphControl;
            _pane = _graph.GraphPane;
            _line = _pane.AddCurve("Line", _points, Color.Blue, SymbolType.Circle);
            _lastXMax = DateTime.Now + _windowWidth;
            _pane.XAxis.Type = AxisType.Date;
            _pane.XAxis.Scale.Format = "MM.ss.fff";
            _pane.XAxis.Scale.MinorStep = 1;
            _pane.XAxis.Scale.MajorStep = 0.2;
            _pane.XAxis.Scale.Min = new XDate(DateTime.Now);
            _pane.XAxis.Scale.Max = new XDate(_lastXMax);
            _pane.X2Axis.Scale.Min = new XDate(DateTime.Now);
            _pane.X2Axis.Scale.Max = new XDate(_lastXMax);
            _lineSin = _pane.AddCurve("Sin", _pointsSin, Color.Red, SymbolType.Plus);
            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            // В противном случае на рисунке будет показана только часть графика,
            // которая умещается в интервалы по осям, установленные по умолчанию
            _graph.AxisChange();

            // Обновляем график
            _graph.Invalidate();
            Task.Factory.StartNew(() => updater.Start(signal, TimeSpan.FromMilliseconds(100), cancel), cancel);
            Task.Factory.StartNew(() => updaterSin.Start(signalSin, TimeSpan.FromMilliseconds(100), cancel), cancel);
        }

        private void UpdaterOnNewMeasure(object sender, MeasureEvent measureEvent)
        {
            this.Dispatcher.Invoke(() =>
            {
                UpdateLine(_points, measureEvent);
            });
        }
        private void UpdaterSinOnNewMeasure(object sender, MeasureEvent measureEvent)
        {
            this.Dispatcher.Invoke(() =>
            {
                UpdateLine(_pointsSin, measureEvent);
            });
        }

        private void UpdateLine(PointPairList points, MeasureEvent measureEvent)
        {
            points.Add(new PointPair(new XDate(measureEvent.Time), measureEvent.Data));
            if (points.Count > 1000)
            {
                points.RemoveAt(0);
            }
            if (_lastXMax < measureEvent.Time)
            {
                _lastXMax = measureEvent.Time;
                _pane.XAxis.Scale.Min = new XDate(_lastXMax - _windowWidth);
                _pane.XAxis.Scale.Max = new XDate(_lastXMax);
                _pane.X2Axis.Scale.Min = new XDate(_lastXMax - _windowWidth);
                _pane.X2Axis.Scale.Max = new XDate(_lastXMax);
            }
            _graph.AxisChange();
            _graph.Invalidate();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _cancellation.Cancel();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
