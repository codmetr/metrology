using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Checks;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces.Checks
{
    public interface ICheckModelFactory
    {
        IMethodViewModel GetModel(CheckConfig checkConfig, ITransportChannelType checkDeviceChanel,
            ITransportChannelType ethalonChanel);
    }
}
