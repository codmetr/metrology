using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADTS;
using IEEE488;
using KipTM.Interfaces.Channels;
using KipTM.Model.TransportChannels;

namespace VisaChannel
{
    public class ChannelsFactory : IChannelsFactory
    {
        public IEnumerable<ITransportChannelType> GetChannels()
        {
            var channels = new ITransportChannelType[]
            {
                new VisaChannelDescriptor(),
                new FakeChannelDescriptor(),
            };
            return channels;
        }

        public Dictionary<string, IDeviceConfig> GetDevicesConfig()
        {
            var channels = new Dictionary<string, IDeviceConfig>()
            {
                {VisaChannelDescriptor.KeyType, new VisaFactory()},
                {FakeChannelDescriptor.KeyType, new VisaFakeFactory()},
            };
            return channels;
        }
    }
}
