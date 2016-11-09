using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Devices
{
    /// <summary>
    /// Строковый описательть единиц измерения
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnitDescriptor<T>
    {
        public UnitDescriptor(T unit, string unitString)
        {
            UnitString = unitString;
            Unit = unit;
        }

        public T Unit { get; private set; }
        public string UnitString { get; private set; }
    }
}
