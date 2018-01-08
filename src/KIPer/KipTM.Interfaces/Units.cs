using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ArchiveData.DTO;

namespace KipTM.Interfaces
{
    public enum Units
    {
        bar = 1,
        mbar = 2,
        kgSm = 3,
        mmHg = 4,
        mA = 101,
        A = 102,
        mV = 201,
        V = 202
    }

    public static class UnitDict
    {
        private static readonly Dictionary<ChannelType, IEnumerable<Units>> _units =
            new Dictionary<ChannelType, IEnumerable<Units>>()
            {
                {ChannelType.Current, new []{Units.A, Units.mA, } },
                {ChannelType.Pressure, new []{Units.bar, Units.mbar, Units.kgSm, Units.mmHg} },
                {ChannelType.Voltage, new []{Units.V, Units.mV, } },
            };

        private static readonly Dictionary<ChannelType, IDictionary<Units, double>> _weights =
            new Dictionary<ChannelType, IDictionary<Units, double>>()
            {
                {ChannelType.Current, new Dictionary<Units, double>()
                {
                    { Units.A, 1.0},
                    { Units.mA, 0.001}
                } },
                {ChannelType.Pressure, new Dictionary<Units, double>()
                {
                    { Units.bar, 1.0},
                    { Units.mbar, 0.001},
                    { Units.kgSm, 98066.5},
                    { Units.mmHg, 133.322},
                } },
                {ChannelType.Voltage, new Dictionary<Units, double>()
                {
                    { Units.V, 1.0},
                    { Units.mV, 0.001}
                } },
            };

        public static readonly Dictionary<Units, string> Default = new Dictionary<Units, string>()
        {
            {Units.bar, "бар" },
            {Units.mbar, "мбар" },
            {Units.kgSm, "кгСм" },
            {Units.mmHg, "мм рт.ст." },
            {Units.mA, "мА" },
            {Units.A, "А" },
            {Units.mV, "мВ" },
            {Units.V, "М" },
        };

        /// <summary>
        /// Локализованный вывод
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToStringLocalized(this Units unit, CultureInfo culture)
        {
            return Default[unit];
        }

        /// <summary>
        /// Получить разобранное значение Units
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static Units ParceLocalized(string unit, CultureInfo culture)
        {
            return Default.FirstOrDefault(el => el.Value == unit).Key;
        }

        /// <summary>
        /// Получить набор поддерживаемых единиц измерений для заданного типа канала
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static IEnumerable<Units> GetUnitsForType(ChannelType channel)
        {
            return _units[channel];
        }

        /// <summary>
        /// Конвертировать значение
        /// </summary>
        /// <param name="channel">тип значения</param>
        /// <param name="val">значение в исходных единицах измерения</param>
        /// <param name="from">исходные единицы измерения</param>
        /// <param name="to">целевые единицы измерения</param>
        /// <returns>сконвертированное значение</returns>
        public static double Convert(ChannelType channel, double val, Units from, Units to)
        {
            var weightFrom = _weights[channel][from];
            var weightTo = _weights[channel][to];
            return (val * weightFrom) / weightTo;
        }
    }
}
