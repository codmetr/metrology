using System;
using System.Threading;
using KipTM.Model.Checks;
using KipTM.Model.Devices;

namespace KipTM.Model.Channels
{
    public class PACEEchalonChannel : IEthalonChannel
    {
        public static string Key = "PACEChannel";

        private readonly PACE1000Model _paseModel;

        public PACEEchalonChannel(PACE1000Model paseModel)
        {
            _paseModel = paseModel;
        }

        public bool Activate()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public double GetEthalonValue(double point, CancellationToken cancel)
        {
            var result = double.NaN;
            result = _paseModel.GetPressure();
            return result;
        }
    }
}
