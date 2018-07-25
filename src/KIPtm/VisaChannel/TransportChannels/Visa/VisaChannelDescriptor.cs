using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisaChannel.TransportChannels.Visa;

namespace KipTM.Model.TransportChannels
{
    [DebuggerDisplay("{Key}({Name})")]
    public class VisaChannelDescriptor: ITransportChannelType
    {
        public static string KeyType = "Visa";
        private VisaSettings _settings;

        public VisaChannelDescriptor()
        {
            Name = "Канал VISA";
            Key = KeyType;
            _settings = new VisaSettings();
        }

        public string Key { get; private set; }
        public string Name { get; private set; }

        public object Settings
        {
            get { return _settings; }
        }
    }
}
