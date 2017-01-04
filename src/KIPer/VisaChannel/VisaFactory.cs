using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEEE488;
using KipTM.Interfaces.Channels;
using KipTM.Model.TransportChannels;
using VisaChannel.TransportChannels.Visa;

namespace VisaChannel
{
    /// <summary>
    /// фабрика драйвера VISA
    /// </summary>
    class VisaFactory : IChannelFactory
    {
        /// <summary>
        /// Получить драйвер VISA
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public object GetDriver(object opt)
        {
            var visaSettings = opt as VisaSettings;
            if (visaSettings != null)
            {
                var transport = new VisaIEEE488();
                transport.Open(visaSettings.AddressFull);
                return transport;
            }
            throw new Exception(string.Format("Can not generate transport for key \"{0}\" with options [{1}]",
                VisaChannelDescriptor.KeyType, opt));
        }
    }
}
