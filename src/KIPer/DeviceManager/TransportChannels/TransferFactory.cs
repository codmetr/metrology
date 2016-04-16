using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeviceManager.TransportChannels.Visa;

namespace DeviceManager.TransportChannels
{
    internal class TransferFactory
    {
        public object GetChannel(ITransportChannelDescriptor key, object settings)
        {
            if (key.Key == VisaChannelDescriptor.KeyType)
            {
                var visasettings = settings as VisaSettings;
                if (visasettings == null)
                    return null;
                return new IEEE488.VisaIEEE488();
            }
            return null;
        }
    }
}
