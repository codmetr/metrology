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
        private TimeSpan _newest = TimeSpan.MinValue;
        private TimeSpan _bigestStart = TimeSpan.MinValue;

        public Scaller(ZedGraphControl zGraph)
        {
            _zGraph = zGraph;
            
        }

        public void Update(TimeSpan newTime, TimeSpan range)
        {
            bool invalidate = CheckScale(newTime, range);

            if (invalidate)
                InvalidateScale(_bigestStart, _newest);

            // Подпишемся на сообщение, уведомляющее о том,
            // что пользователь изменяет масштаб графика
            _zGraph.ZoomEvent += new ZedGraphControl.ZoomEventHandler(ZoomUserChanged);

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            _zGraph.AxisChange();

            // Обновляем график
            _zGraph.Invalidate();
        }

        private void ZoomUserChanged(ZedGraphControl sender, ZoomState oldstate, ZoomState newstate)
        {

            GraphPane pane = sender.GraphPane;

            // Для простоты примера будем ограничивать масштабирование
            // только в сторону уменьшения размера графика

            // Проверим интервал для каждой оси и
            // при необходимости скорректируем его

            if (pane.XAxis.Scale.Min <= -100)
            {
                pane.XAxis.Scale.Min = -100;
            }

            if (pane.XAxis.Scale.Max >= 100)
            {
                pane.XAxis.Scale.Max = 100;
            }

            if (pane.YAxis.Scale.Min <= -1)
            {
                pane.YAxis.Scale.Min = -1;
            }

            if (pane.YAxis.Scale.Max >= 2)
            {
                pane.YAxis.Scale.Max = 2;
            }
        }

        private bool CheckScale(TimeSpan newTime, TimeSpan range)
        {
            bool invalidate = false;
            if (_bigestStart == TimeSpan.MinValue)
            {
                _bigestStart = newTime;
                _newest = newTime + range;
                return true;
            }

            var start = newTime - range;
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

            return invalidate;
        }

        private void InvalidateScale(TimeSpan start, TimeSpan end)
        {
            // Получим панель для рисования
            GraphPane pane = _zGraph.GraphPane;
            if(pane == null)
                return;
            pane.XAxis.Scale.Min = new XDate(DateTime.MinValue + start);
            pane.XAxis.Scale.Max = new XDate(DateTime.MinValue + end);
        }
    }
}
