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
        private VisaIEEE488 _visa = null;
        private readonly int _address = 0;

        public PaceEtalonSource(int address)
        {
            _address = address;
        }

        public bool SetEtalonValue(double aim, Units unit, CancellationToken cancel)
        {
            //TODO реализовать установку единиц измерения
            var presUnit = ConvertUnit(unit);
            _paseModel.SetPressureUnit(presUnit);
            return _paseModel.SetPressure(aim);
        }

        private PressureUnits ConvertUnit(Units unit)
        {
            switch (unit)
            {
                case Units.bar:
                    return PressureUnits.Bar;
                case Units.mbar:
                    return PressureUnits.MBar;
                case Units.kgSm:
                    return PressureUnits.KgCm2;
                case Units.mmHg:
                    return PressureUnits.mmHg;
                case Units.mA:
                case Units.A:
                case Units.mV:
                case Units.V:
                  default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }

        public bool Activate(ITransportChannelType transport)
        {
            if (transport.Key != VisaChannelDescriptor.KeyType)
                throw new TranspotrTypeNotAvailableException(string.Format("Channel {0} not available for PACE",
                    transport.Key));
            _visa = new VisaIEEE488();
            _paseModel = new PACE1000Driver(_visa);
            return _visa.Open(_address);
        }

        public void Stop()
        {
            _paseModel.Dispose();
            _visa.Close();
        }

        public bool SetEtalonValue(double aim, CancellationToken cancel)
        {
            return _paseModel.SetPressure(aim);
        }
    }
}
