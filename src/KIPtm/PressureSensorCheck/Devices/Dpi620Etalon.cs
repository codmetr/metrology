using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchiveData.DTO;
using DPI620Genii;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Model.TransportChannels;

namespace PressureSensorCheck.Devices
{
    public class Dpi620Etalon: IEtalonChannel
    {
        private readonly IDPI620Driver _dpi620;
        private readonly int _slotId;
        private readonly ChannelType _channelType;
        private readonly Units _unitFrom;
        private readonly Units _unitTo;

        /// <summary>
        /// Эталонный канал на основе DPI620Genii
        /// </summary>
        /// <param name="dpi620">двайвер DPI620Genii</param>
        /// <param name="slotId">номер слота (с 1), по которому подключено оборудование</param>
        /// <param name="channelType">тип канала</param>
        /// <param name="unitFrom">тип канала</param>
        /// <param name="unitTo">ожидаемые единицы измерения</param>
        public Dpi620Etalon(IDPI620Driver dpi620, int slotId, ChannelType channelType, Units unitFrom, Units unitTo)
        {
            _dpi620 = dpi620;
            _slotId = slotId;
            _channelType = channelType;
            _unitFrom = unitFrom;
            _unitTo = unitTo;
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

        public double GetEtalonValue(double point, CancellationToken calcel)
        {
            var val = _dpi620.GetValue(_slotId);
            var realVal = UnitDict.Convert(_channelType, val, _unitFrom, _unitTo);
            return realVal;
        }
    }
}
