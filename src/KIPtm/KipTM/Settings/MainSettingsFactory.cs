using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Settings
{
    public class MainSettingsFactory
    {
        private readonly IEnumerable<IDeviceSettingsFactory> _deviceFactories;
        private readonly IEnumerable<IEtalonSettingsFactory> _etalonFactories;
        private readonly IEnumerable<IDeviceTypeSettingsFactory> _deviceTypeFactories;

        public MainSettingsFactory(IEnumerable<IDeviceSettingsFactory> deviceFactories,
            IEnumerable<IEtalonSettingsFactory> etalonFactories, IEnumerable<IDeviceTypeSettingsFactory> deviceTypeFactories)
        {
            _deviceFactories = deviceFactories;
            _etalonFactories = etalonFactories;
            _deviceTypeFactories = deviceTypeFactories;
        }


        public MainSettings GetDefault()
        {
            var res = new MainSettings();

            var ports = SerialPort.GetPortNames().Select(el => new ComPortSettings()
            {
                Name = el,
                NumberCom = 1,
                Rate = 9600,
                Parity = Parity.None,
                CountBits = 8,
                CountStopBits = StopBits.One
            }).ToList();
            res.Ports = ports;

            res.Devices = _deviceTypeFactories.SelectMany(el => el.GetDefault()).ToList();
            res.LastEtalons = _etalonFactories.Select(el => el.GetDefault()).ToList();
            res.LastDevices = _deviceFactories.Select(el => el.GetDefault()).ToList(); ;
            return res;
        }
    }
}
