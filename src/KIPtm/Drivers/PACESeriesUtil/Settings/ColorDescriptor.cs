using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PACESeriesUtil.VM
{
    /// <summary>
    /// Описатель цвета
    /// </summary>
    public class ColorDescriptor
    {
        private readonly string _name;
        private readonly Color _color;

        /// <summary>
        /// Описатель цвета
        /// </summary>
        /// <param name="name">Название цвет</param>
        /// <param name="color">Цвет</param>
        public ColorDescriptor(string name, Color color)
        {
            _name = name;
            _color = color;
        }

        /// <summary>
        /// Название цвет
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Цвет
        /// </summary>
        public Color ColorVal
        {
            get { return _color; }
        }
    }
}
