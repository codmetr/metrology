using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckFrame.Model.TransportChannels;

namespace KipTM.Model.TransportChannels
{
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
