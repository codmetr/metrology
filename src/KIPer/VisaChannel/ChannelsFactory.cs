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
    public class ChannelsFactory
    {
        public IEnumerable<ITransportChannelType> GetChannelTypes()
        {
            var channels = new ITransportChannelType[]
            {
                new VisaChannelDescriptor(),
                new FakeChannelDescriptor(),
            };
            return channels;
        }

        public Dictionary<string, IChannelFactory> GetChannels()
        {
            var channels = new Dictionary<string, IChannelFactory>()
            {
                {VisaChannelDescriptor.KeyType, new VisaFactory()},
                {FakeChannelDescriptor.KeyType, new VisaFakeFactory()},
            };
            return channels;
        }
    }
}
