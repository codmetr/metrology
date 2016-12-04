using System.Reflection;
using CheckFrame.Checks;
using IEEE488;
using KipTM.Interfaces.Checks;
using PACESeries;

namespace KipTM.ViewModel.Checks
{
    [DeviceFactoryAttribute(typeof(PACE1000Driver))]
    public class PACE1000Factory : IDeviceFactory
    {
        public object GetDevice(object options)
        {
            var param = options as ITransportIEEE488;
            if (param == null)
                throw new TargetParameterCountException(string.Format(
                    "option mast be type: {0}; now type: {1}",
                    typeof(ITransportIEEE488), options.GetType()));
            return new PACE1000Driver(param);
        }
    }
}
