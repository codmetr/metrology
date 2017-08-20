using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckFrame.Checks;
using DPI620Genii;
using KipTM.Interfaces.Checks;

namespace PressureSensorCheck.Devices
{
    [DeviceFactory(typeof(DPI620DriverUsb))]
    public class DPI620Factory : IDeviceFactory
    {
        public object GetDevice(object options)
        {
            return new DPI620DriverUsb();
        }
    }
}
