using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPI620Genii
{
    /// <summary>
    /// Дескриптор канала
    /// </summary>
    public class ChannelDescriptor
    {
        public ChannelDescriptor(int number, string typeId, ChannelOrder order, string unit, double range)
        {
            Number = number;
            TypeId = typeId;
            Order = order;
            Unit = unit;
            Range = range;
        }

        /// <summary>
        /// Номер канала
        /// </summary>
        public int Number { get; private set; }
        /// <summary>
        /// Идентификатор типа канала
        /// </summary>
        public string TypeId { get; private set; }
        /// <summary>
        /// Нарпаленность канала
        /// </summary>
        public ChannelOrder Order { get; private set; }
        /// <summary>
        /// Единицы измерения канала
        /// </summary>
        public string Unit { get; private set; }
        /// <summary>
        /// Диапазон по каналу
        /// </summary>
        public double Range { get; private set; }
    }
}
