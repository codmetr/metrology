﻿using System;
using System.Collections.Generic;
using ArchiveData.DTO;
using CheckFrame.Model;
using CheckFrame.Model.Checks;
using KipTM.Interfaces;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Settings;

namespace KipTM.Design
{
    public class DesignDataService : IDataService
    {
        private IDeviceManager _deviceManager = null;

        public IDeviceManager DeviceManager
        {
            get { return _deviceManager; }
        }

        public IEnumerable<IDeviceTypeDescriptor> DeviceTypes { get; private set; }
        public IEnumerable<IDeviceTypeDescriptor> EtalonTypes { get; private set; }
        public IEnumerable<DeviceDescriptor> Etalons { get; private set; }

        public IDictionary<string, ICheckMethod> Methodics { get; private set; }
        public ResultsArchive ResultsArchive { get; private set; }
        public IMainSettings Settings { get; private set; }

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

        public void FillDeviceList(IEnumerable<DeviceTypeDescriptor> deviceTypes, IEnumerable<DeviceTypeDescriptor> etalonTypes)
        {
            throw new NotImplementedException();
        }
    }
}