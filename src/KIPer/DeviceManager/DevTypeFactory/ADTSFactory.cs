using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADTS;
using DeviceManager.TransportChannels;
using IEEE488;

namespace DeviceManager.DevTypeFactory
{
    internal class ADTSFactory
    {
        public ADTSDriverByCommonChannel GetDevice(ITransportChannelDescriptor channel, ITransportIEEE488 channel)
        {
            return new ADTSDriverByCommonChannel(new ADTSDriver(channel));
        }
    }
}
