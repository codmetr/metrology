using System;
using System.Collections.Generic;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Archive;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Settings;

namespace KipTM.Design
{
    public class DesignDataService : IDataService
    {
        private IDeviceManager _deviceManager;

        public IDeviceManager DeviceManager
        {
            get { return _deviceManager; }
        }

        public IEnumerable<ICheckMethodic> Methodics { get; private set; }
        public ResultsArchive ResultsArchive { get; private set; }
        public MainSettings Settings { get; private set; }

        public void LoadSettings()
        {
            //throw new NotImplementedException();
        }

        public void SaveSettings()
        {
            throw new NotImplementedException();
        }

        public void LoadResults()
        {
            throw new NotImplementedException();
        }

        public void SaveResults()
        {
            throw new NotImplementedException();
        }

        public void InitDevices()
        {
            //throw new NotImplementedException();
        }
    }
}