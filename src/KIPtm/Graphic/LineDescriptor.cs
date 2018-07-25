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
        public LineDescriptor()
        {
            Width = 1;
        }
        /// <summary>
        /// Название линии
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название шкалы
        /// </summary>
        public string AzixTitle { get; set; }

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

        /// <summary>
        /// Ширина линии
        /// </summary>
        public double Width { get; set; }
    }

    /// <summary>
    /// Описатель конкретной точки
    /// </summary>
    public class PointData
    {
        /// <summary>
        /// Время
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public double Value { get; set; }
    }
}
