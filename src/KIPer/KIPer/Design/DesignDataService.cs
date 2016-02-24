using System;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;

namespace KipTM.Design
{
    public class DesignDataService : IDataService
    {
        private IDeviceManager _deviceManager;

        public IDeviceManager DeviceManager
        {
            get { return _deviceManager; }
        }

        public void LoadSettings()
        {
            //throw new NotImplementedException();
        }

        public void SaveSettings()
        {
            throw new NotImplementedException();
        }

        public void InitDevices()
        {
            //throw new NotImplementedException();
        }
    }
}