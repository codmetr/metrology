﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace Graphic
{
    /// <summary>
    /// Отписыватель от конкретной линии
    /// </summary>
    class LineHolder
    {
        private readonly GraphPane _pane;
        private readonly PointPairList _list;
        private readonly LineItem _curve;
        private readonly LineDescriptor _line;
        private readonly Scaller _scaller;

        private LineHolder(PointPairList list, LineItem curve, LineDescriptor line, GraphPane pane, Scaller scaller)
        {
            _list = list;
            _curve = curve;
            _line = line;
            _pane = pane;
            _scaller = scaller;
        }

        public static LineHolder Hold(GraphPane pane, LineDescriptor line, Scaller scaller)
        {
            PointPairList list = new PointPairList();
            // Создадим кривую с названием "Sinc", 
            // которая будет рисоваться голубым цветом (Color.Blue),
            // Опорные точки выделяться не будут (SymbolType.None)
            LineItem curve = pane.AddCurve(line.Title, list, line.LineColor, SymbolType.None);
            var holder = new LineHolder(list, curve, line, pane, scaller);
            holder.Attach();
            return holder;
        }

        public static void Free(LineHolder holder)
        {
            holder._pane.CurveList.Remove(holder._curve);
            holder.Detach();
        }

        private void Attach()
        {
            _line.Source.CollectionChanged += CollectionChanged;
        }

        public void Detach()
        {
            _line.Source.CollectionChanged -= CollectionChanged;
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                var point = item as PointData;
                if(point == null)
                    continue;
                _list.Add(new XDate(point.Time), point.Value);
                _scaller.Update(point.Time, _line.LimitForLine);
            }
        }
    }
}
