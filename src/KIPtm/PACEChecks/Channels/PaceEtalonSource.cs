using System;
using System.Threading;
using System.Threading.Tasks;
using IEEE488;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Exceptions;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;
using PACEChecks.Devices;
using PACESeries;

namespace PACEChecks.Channels
{
    public class PaceEtalonSource : IEtalonSourceChannel<Units>
    {
        private PACE1000Driver _paseModel = null;
        private readonly int _address = 0;

        public PaceEtalonSource(int address)
        {
            _address = address;
        }

        public bool SetEtalonValue(double aim, Units unit, CancellationToken cancel)
        {
            //TODO реализовать установку единиц измерения
            return _paseModel.SetPressure(aim);
        }

        public bool Activate(ITransportChannelType transport)
        {
            if (transport.Key != VisaChannelDescriptor.KeyType)
                throw new TranspotrTypeNotAvailableException(string.Format("Channel {0} not available for PACE",
                    transport.Key));
            _paseModel = new PACE1000Driver(_address, new VisaIEEE488());
            return _paseModel.Open();
        }

        public void Stop()
        {
            _paseModel.Dispose();
        }

        public bool SetEtalonValue(double aim, CancellationToken cancel)
        {
            return _paseModel.SetPressure(aim);
        }
    }
}
