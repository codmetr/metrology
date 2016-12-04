using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveData.DTO;
using CheckFrame.Checks;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Checks;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces.Checks
{

    public interface IDeviceFactory
    {
        object GetDevice(object options);
    }
}
