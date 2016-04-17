using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace IEEE488
{
    public class TransportIEEE488 : ITransportIEEE488
    {
        private SerialPort _port;

        public TransportIEEE488(SerialPort port)
        {
            _port = port;
        }

        public bool Open(int address)
        {
            throw new NotImplementedException();
        }

        public bool Close(int address)
        {
            throw new NotImplementedException();
        }

        public bool Send(string data)
        {
            throw new NotImplementedException();
        }

        public string Receive()
        {
            throw new NotImplementedException();
        }
    }
}
