using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPI620Genii
{
    class ParamId
    {
        public ParamId(Enum unit, ParamType parameterType, int unitNumber, int unitId)
        {
            Unit = unit;
            ParameterType = parameterType;
            UnitNumber = unitNumber;
            UnitId = unitId;
        }

        public enum ParamType
        {
        Humidity,
        Temperature,
        TemperatureRTD,
        TemperatureTC,
        Pressure,
        Frequency,
        Density,
        Resistance,
        Current,
        Voltage,
        Observed,
        }

        public Enum Unit { get; private set; }

        public ParamType ParameterType { get; private set; }

        public int UnitNumber { get; private set; }

        public int UnitId { get; private set; }
    }
}
