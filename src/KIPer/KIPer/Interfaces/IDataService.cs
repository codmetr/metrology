using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KipTM.Model;
using KipTM.Model.Archive;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Settings;

namespace KipTM.Interfaces
{
    public interface IDataService
    {
        IDeviceManager DeviceManager { get; }
        IEnumerable<ICheckMethodic> Methodics { get; }
        ResultsArchive ResultsArchive { get; }
        MainSettings Settings { get; }
        void LoadSettings();
        void SaveSettings();
        void LoadResults();
        void SaveResults();
        void InitDevices();
    }
}
