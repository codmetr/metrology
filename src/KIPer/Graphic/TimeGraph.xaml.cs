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

        private void Detach(IEnumerable<LineDescriptor> collection)
        {
            foreach (var holder in _holders)
            {
                LineHolder.Free(holder);
            }
        }
    }
}
