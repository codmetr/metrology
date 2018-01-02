using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;

namespace KipTM.Interfaces
{
    public enum Units
    {
        bar,
        kgSm,
        mA,
        A,
        mV,
        V
    }

    public static class UnitDict
    {
        private static readonly Dictionary<ChannelType, IEnumerable<Units>> _units =
            new Dictionary<ChannelType, IEnumerable<Units>>()
            {
                {ChannelType.Current, new []{Units.A, Units.mA, } },
                {ChannelType.Pressure, new []{Units.bar, Units.kgSm, } },
                {ChannelType.Voltage, new []{Units.V, Units.mV, } },
            };

        public static IEnumerable<Units> GetUnitsForType(ChannelType channel)
        {
            return _units[channel];
        }
    }
}
