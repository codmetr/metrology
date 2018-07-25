using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZedGraph;

namespace Graphic
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TimeGraph : UserControl
    {
        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static readonly DependencyProperty LinesProperty = DependencyProperty.Register(
            "Lines", typeof(IEnumerable<LineDescriptor>), typeof(TimeGraph), new PropertyMetadata(default(IEnumerable<LineDescriptor>), LinesChanged));

        private static void LinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graph = d as TimeGraph;
            if(graph == null)
                return;
            var collection = e.NewValue as IEnumerable<LineDescriptor>;
            if(collection == null)
                return;
            graph.UpdateLines(collection);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(TimeGraph), new PropertyMetadata(default(string), UpdateTitle));

        private static void UpdateTitle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graph = d as TimeGraph;
            if (graph == null)
                return;
            graph._zGraph.GraphPane.Title.Text = e.NewValue.ToString();
        }

        public static readonly DependencyProperty ClanerProperty = DependencyProperty.Register(
            "Claner", typeof(CleanerAct), typeof(TimeGraph), new PropertyMetadata(default(CleanerAct), UpdateCleaner));

        private static void UpdateCleaner(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graph = d as TimeGraph;
            if (graph == null)
                return;
            var state = e.NewValue as CleanerAct;
            if (state != null)
                state.Clear += () => graph.Clear();
        }

        public static readonly DependencyProperty IsLockProperty = DependencyProperty.Register(
            "IsLock", typeof(bool), typeof(TimeGraph), new PropertyMetadata(default(bool), UpdateIsLock));

        private static void UpdateIsLock(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var val = (bool) e.NewValue;
            var graph = d as TimeGraph;
            graph.GraphHost.IsEnabled = !val;
            graph.GraphHost.Child.Enabled = !val;
            graph.GraphHost.Visibility = val ? Visibility.Collapsed : Visibility.Visible;

            if (val)
            {
                Bitmap b = new Bitmap(graph.GraphHost.Child.Width, graph.GraphHost.Child.Height);
                graph.GraphHost.Child.DrawToBitmap(b, new Rectangle(0, 0, b.Width, b.Height));

                var handle = b.GetHbitmap();
                try
                {
                    graph.GraphImmage.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero,
                        Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(handle);
                }
            }
            graph.GraphImmage.Visibility = val ? Visibility.Visible : Visibility.Collapsed;
            Debug.WriteLine($"IsLock:{val}");
        }

        public bool IsLock
        {
            get { return (bool) GetValue(IsLockProperty); }
            set { SetValue(IsLockProperty, value); }
        }

        public CleanerAct Claner
        {
            get { return (CleanerAct) GetValue(ClanerProperty); }
            set { SetValue(ClanerProperty, value); }
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public IEnumerable<LineDescriptor> Lines
        {
            get { return (IEnumerable<LineDescriptor>) GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }

        private IEnumerable<LineDescriptor> _collection;
        private List<LineHolder> _holders;
        private ZedGraphControl _zGraph;
        private Scaller _scaller;

        public TimeGraph()
        {
            InitializeComponent();
            _zGraph = GraphHost.Child as ZedGraphControl;
            _holders = new List<LineHolder>();
            _scaller = new Scaller(_zGraph);
            _zGraph.GraphPane.Title.Text = Title;
            _zGraph.ZoomEvent += ZoomChanged;
            _zGraph.GraphPane.YAxisList.Clear();
        }

        private void ZoomChanged(ZedGraphControl sender, ZoomState oldstate, ZoomState newstate)
        {
            ToBaseScale(sender);
        }

        private void UpdateLines(IEnumerable<LineDescriptor> newCollection)
        {
            if (newCollection == null)
                return;
            if (_collection != null)
                Detach(_collection);
            _collection = newCollection;
            Attach(_collection);
        }

        private void Clear()
        {
            _zGraph.GraphPane.CurveList.Clear();
        }

        private void Attach(IEnumerable<LineDescriptor> collection)
        {
            // Получим панель для рисования
            ToBaseScale(_zGraph);
            GraphPane pane = _zGraph.GraphPane;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            //pane.CurveList.Clear();

            foreach (var descriptor in collection)
            {
                _holders.Add(LineHolder.Hold(pane, descriptor, _scaller));
            }
            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию
            _zGraph.AxisChange();

            // Обновляем график
            _zGraph.Invalidate();
        }

        private void ToBaseScale(ZedGraphControl zGraph)
        {
            GraphPane pane = zGraph.GraphPane;
            pane.XAxis.Type = AxisType.Date;
            pane.XAxis.Scale.Format = "mm.ss.fff";
            pane.XAxis.Scale.MinorStep = 1;
            pane.XAxis.Scale.MajorStep = 0.25;
            
            // Изменим тест надписи по оси X
            pane.XAxis.Title.Text = "Время";

            // Изменим параметры шрифта для оси X
            pane.XAxis.Title.FontSpec.IsBold = false;
        }

        private void Detach(IEnumerable<LineDescriptor> collection)
        {
            foreach (var holder in _holders)
            {
                LineHolder.Free(holder);
            }
            _zGraph.GraphPane.Y2AxisList.Clear();
        }

    }
}
