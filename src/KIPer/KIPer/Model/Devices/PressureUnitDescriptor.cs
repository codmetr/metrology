using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PACESeries;

namespace KipTM.Model.Devices
{
    public class PressureUnitDescriptor
    {
        public PressureUnitDescriptor(PressureUnits unit, string unitString)
        {
            UnitString = unitString;
            Unit = unit;
        }

        public PressureUnits Unit { get; private set; }
        public string UnitString { get; private set; }
    }
}
