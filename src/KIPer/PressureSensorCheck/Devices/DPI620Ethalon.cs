using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DPI620Genii;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Model.TransportChannels;

namespace PressureSensorCheck.Devices
{
    public class DPI620Ethalon: IEthalonChannel
    {
        private readonly IDPI620Driver _dpi620;
        private readonly int _slotId;

        /// <summary>
        /// Эталонный канал на основе DPI620Genii
        /// </summary>
        /// <param name="dpi620">двайвер DPI620Genii</param>
        /// <param name="slotId">номер слота (с 1), по которому подключено оборудование</param>
        public DPI620Ethalon(IDPI620Driver dpi620, int slotId)
        {
            _dpi620 = dpi620;
            _slotId = slotId;
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
            return _dpi620.GetValue(_slotId);
        }
    }
}
