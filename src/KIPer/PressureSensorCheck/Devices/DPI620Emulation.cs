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
            if (slotId == 0)
                return 0.5;
            else if (slotId == 1)
                return 1.5;
            return 2.5;
        }

        public void Open()
        {
            
        }
    }
}
