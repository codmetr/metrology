using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPI620Genii;

namespace PressureSensorCheck.Devices
{
    class DPI620Emulation: IDPI620Driver
    {
        public void Close()
        {
            
        }

        public double GetValue(int slotId)
        {
            var dif = new Random().NextDouble()/4;
            if (slotId == 0)
                return 0d + dif;
            else if (slotId == 1)
                return 1d + dif;
            return 2d + dif;
        }

        /// <inheritdoc />
        public IEnumerable<int> TestSlots()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            
        }
    }
}
