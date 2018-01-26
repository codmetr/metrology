using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace Graphic
{
    /// <summary>
    /// Рассчет общей шкалы
    /// </summary>
    internal class Scaller
    {
        private ZedGraphControl _zGraph;
        private DateTime _newest = DateTime.MinValue;
        private DateTime _bigestStart = DateTime.MinValue;

        public Scaller(ZedGraphControl zGraph)
        {
            _zGraph = zGraph;
        }

        public void Update(DateTime newTime, TimeSpan range)
        {
            if (_bigestStart == DateTime.MinValue)
            {
                _bigestStart = newTime;
                _newest = newTime + range;
            }

            var start = newTime - range;
            bool invalidate = false;
            if (_bigestStart < start)
            {
                _bigestStart = start;
                invalidate = true;
            }
            if (_newest < newTime)
            {
                _newest = newTime;
                invalidate = true;
            }

            if(invalidate)
                InvalidateScale(_bigestStart, _newest);

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            _zGraph.AxisChange();

            // Обновляем график
            _zGraph.Invalidate();
        }

        private void InvalidateScale(DateTime start, DateTime end)
        {
            // Получим панель для рисования
            GraphPane pane = _zGraph.GraphPane;

            pane.XAxis.Scale.Min = new XDate(start);
            pane.XAxis.Scale.Max = new XDate(start);
        }
    }
}
