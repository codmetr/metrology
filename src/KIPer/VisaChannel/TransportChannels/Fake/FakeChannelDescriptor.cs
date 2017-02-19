using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.TransportChannels
{
    [DebuggerDisplay("{Key}({Name})")]
    public class FakeChannelDescriptor: ITransportChannelType
    {
        public static string KeyType = "Fake";

        public FakeChannelDescriptor()
        {
            Name = "Канал пустышка";
            Key = KeyType;
        }

        public string Key { get; private set; }
        public string Name { get; private set; }

        public object Settings
        {
            get { return null; }
        }
    }
}
