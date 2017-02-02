using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckFrame.Channels
{
    /// <summary>
    /// Описатель канала
    /// </summary>
    public class ChannelDescriptor
    {
        /// <summary>
        /// Название канала
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Направленность
        /// </summary>
        public ChannelOrder Order { get; set; }

        /// <summary>
        /// Тип измеряемой величины
        /// </summary>
        public ChannelType TypeChannel { get; set; }

        /// <summary>
        /// Минимум
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Максимум
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Допустимая погрешность
        /// </summary>
        public double Error { get; set; }
    }
}
