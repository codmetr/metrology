using System;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Interfaces.Channels;
using KipTM.Interfaces.Exceptions;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;
using PACEChecks.Devices;
using PACESeries;

namespace PACEChecks.Channels
{
    public class PACEEtalonSourceChannel : IEtalonSourceChannel<double>
    {
        private readonly PACE1000Driver _paseModel;

        public PACEEtalonSourceChannel(PACE1000Driver paseModel)
        {
            _paseModel = paseModel;
        }

        public bool SetEtalonValue(double aim, double unit, CancellationToken cancel)
        {
            return _paseModel.SetPressure(aim);
        }

        public bool Activate(ITransportChannelType transport)
        {
            if (transport.Key != VisaChannelDescriptor.KeyType)
                throw new TranspotrTypeNotAvailableException(string.Format("Channel {0} not available for PACE",
                    transport.Key));
            return _paseModel.Open();
        }

        public void Stop()
        {
            _paseModel.Dispose();
        }

        public bool SetEtalonValue(double aim, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}
