using System.Collections.Generic;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces.Channels
{
    public interface IChannelsFactory
    {
        IEnumerable<ITransportChannelType> GetChannels();
    }
}