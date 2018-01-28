using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ZedGraph;

namespace Graphic
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TimeGraph : UserControl
    {
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
