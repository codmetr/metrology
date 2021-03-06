﻿using CheckFrame.Checks;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using MainLoop;

namespace PACEChecks.Devices
{
    [DeviceModelFactory(typeof(PACE1000Model))]
    public class PACE1000ModelFactory : IDeviceModelFactory
    {
        public object GetModel(ILoops loops, IDeviceManager deviceManager)
        {
            return new PACE1000Model(PACE1000Model.Model, loops, deviceManager);
        }
    }

    //public 
}
