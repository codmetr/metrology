using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.TransportChannels;

namespace KipTM.DataService
{
    class DeviceCacheKey
    {
        public DeviceCacheKey(Type deviceType, ITransportChannelType channel)
        {
            DeviceType = deviceType;
            Channel = channel;
        }

        public Type DeviceType { get; private set; }
        public ITransportChannelType Channel { get; private set; }
    }
}
