﻿using ADTSChecks.Devices;
using CheckFrame.Checks;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using MainLoop;

namespace KipTM.ViewModel.Checks
{
    [DeviceModelFactory(typeof(ADTSModel))]
    public class ADTSModelFactory : IDeviceModelFactory
    {
        public object GetModel(ILoops loops, IDeviceManager deviceManager)
        {
            return new ADTSModel(ADTSModel.Model, loops, deviceManager);
        }
    }
}
