using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Device;

namespace PACESeries
{
    class PACEModbusDriver:PACEDriver
    {
        private IModbusMaster master;
        public PACEModbusDriver(int address) : base(address)
        {
        }

        protected override void Send(string command)
        {

        }

        protected override string Receive()
        {
            return null;
        }
    }
}
