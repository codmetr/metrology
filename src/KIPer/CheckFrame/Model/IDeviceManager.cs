using CheckFrame.Model.Channels;
using CheckFrame.Model.TransportChannels;

namespace CheckFrame.Model
{
    public interface IDeviceManager
    {
        IEthalonChannel GetEthalonChannel(string deviceKey, ITransportChannelType settings);

        T GetDevice<T>(ITransportChannelType transportDescription);

        T GetModel<T>();
    }
}