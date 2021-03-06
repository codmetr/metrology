﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Devices
{
    public class PressureUnitDescriptor<T>
    {
        public PressureUnitDescriptor(T unit, string unitString)
        {
            UnitString = unitString;
            Unit = unit;
        }

        public T Unit { get; private set; }
        public string UnitString { get; private set; }
    }
}
