using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DPI620Genii;
using KipTM.Interfaces.Channels;
using KipTM.Model.TransportChannels;

namespace PressureSensorCheck.Devices
{
    public class DPI620Ethalon: IEthalonChannel
    {
        private readonly IDPI620Driver _dpi620;
        private readonly int _slotId;
        private readonly string _unit;

        public DPI620Ethalon(IDPI620Driver dpi620, int slotId, string unit)
        {
            _dpi620 = dpi620;
            _slotId = slotId;
            _unit = unit;
        }

        public bool Activate(ITransportChannelType transport)
        {
            try
            {
                _dpi620.Open();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public void Stop()
        {
            _dpi620.Close();
        }

        public double GetEthalonValue(double point, CancellationToken calcel)
        {
            return _dpi620.GetValue(_slotId, _unit);
        }
    }
}
