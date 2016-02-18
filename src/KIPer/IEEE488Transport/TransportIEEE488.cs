using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEEE488
{
    public class TransportIEEE488 : ITransportIEEE488
    {
        public bool Open(int address)
        {
            throw new NotImplementedException();
        }

        public bool Close(int address)
        {
            throw new NotImplementedException();
        }

        public bool Send(int address, string data)
        {
            throw new NotImplementedException();
        }

        public string Receive(int address)
        {
            throw new NotImplementedException();
        }
    }
}
