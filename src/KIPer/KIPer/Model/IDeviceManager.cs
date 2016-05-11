using System.Collections.Generic;
using KipTM.Model.Channels;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;

namespace KipTM.Model
{
    public interface IDeviceManager
    {
        IEthalonChannel GetEthalonChannel(string deviceKey, ITransportChannelType settongs);

        T GetDevice<T>(ITransportChannelType transportDescription);

        T GetModel<T>();
    }
}