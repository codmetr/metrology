using System.Reflection;
using ADTS;
using ADTSChecks.Model.Devices;
using CheckFrame.Checks;
using IEEE488;
using KipTM.Interfaces.Checks;

namespace KipTM.ViewModel.Checks
{
    [DeviceFactoryAttribute(typeof(ADTSDriver))]
    public class ADTSFactory : IDeviceFactory
    {
        public object GetDevice(object options)
        {
            var param = options as ITransportIEEE488;
            if (param == null)
                throw new TargetParameterCountException(string.Format(
                    "option mast be type: {0}; now type: {1}",
                    typeof(ITransportIEEE488), options.GetType()));
            return new ADTSDriver(param);
        }
    }
}
