using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphic
{
    /// <summary>
    /// Описатель линии графа
    /// </summary>
    public class LineDescriptor
    {
        /// <summary>
        /// Название линии
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Цвет линии
        /// </summary>
        public Color LineColor { get; set; }

        /// <summary>
        /// Источник данных
        /// </summary>
        public INotifyCollectionChanged Source { get; set; }

        /// <summary>
        /// Ограничения для линии
        /// </summary>
        public TimeSpan LimitForLine { get; set; }
    }

    /// <summary>
    /// Описатель конкретной точки
    /// </summary>
    public class PointData
    {
        /// <summary>
        /// Время
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public double Value { get; set; }
    }
}
