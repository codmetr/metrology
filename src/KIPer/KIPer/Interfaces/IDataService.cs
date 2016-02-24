using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KipTM.Model;
using KipTM.Model.Devices;

namespace KipTM.Interfaces
{
    public interface IDataService
    {
        IDeviceManager DeviceManager { get; }
        void LoadSettings();
        void SaveSettings();
        void InitDevices();
    }
}
