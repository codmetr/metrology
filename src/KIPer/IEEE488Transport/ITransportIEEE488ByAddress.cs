using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEEE488
{
    public interface ITransportIEEE488ByAddress
    {
        bool Open(int address);
        bool Close(int address);
        bool Send(int address, string data);
        string Receive(int address);
    }
}
