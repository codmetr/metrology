using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSChecks.Model.Devices;
using KipTM.Model.Checks;

namespace KipTM.Settings
{
    public class MainSettings : IMainSettings
    {
        public const string SettingsFileName = "settings";

        public static MainSettings GetDefault()
        {
            var res = new MainSettings();
            res.Ports = new List<ComPortSettings>()
            {
                new ComPortSettings()
                {
                    Name = "COM1",
                    NumberCom = 1,
                    Rate = 9600,
                    Parity = Parity.None,
                    CountBits = 8,
                    CountStopBits = StopBits.One
                },
                new ComPortSettings()
                {
                    Name = "COM2",
                    NumberCom = 1,
                    Rate = 9600,
                    Parity = Parity.None,
                    CountBits = 8,
                    CountStopBits = StopBits.One
                }
            };
            res.Devices = new List<DeviceTypeSettings>()
            {
                new DeviceTypeSettings()
                {
                    Key = ADTSModel.Key,
                    Model = ADTSModel.Model,
                    DeviceCommonType = ADTSModel.DeviceCommonType,
                    DeviceManufacturer = ADTSModel.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(ADTSModel.TypesEtalonParameters),
                    //AvilableEthalonTypes = new List<string>(){KipTM.Model.Devices.PACE5000Model.Key, UserEchalonChannel.Key},
                },
                new DeviceTypeSettings()
                {
                    Key = PACE1000Model.Key,
                    Model = PACE1000Model.Model,
                    DeviceCommonType = PACE1000Model.DeviceCommonType,
                    DeviceManufacturer = PACE1000Model.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(ADTSModel.TypesEtalonParameters),
                    //AvilableEthalonTypes = new List<string>(){KipTM.Model.Devices.PACE5000Model.Key, UserEchalonChannel.Key},
                },

            };
            res.LastEtalons = new List<DeviceSettings>()
            {
                new DeviceSettings()
                {
                    Address = "0",
                    Name = PACE1000Model.Key,
                    Model = PACE1000Model.Model,
                    DeviceCommonType = PACE1000Model.DeviceCommonType,
                    DeviceManufacturer = PACE1000Model.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(PACE1000Model.TypesEtalonParameters),
                    SerialNumber = "123",
                    NamePort = "COM1"
                },
            };
            res.LastDevices = new List<DeviceSettings>()
            {
                new DeviceSettings()
                {
                    Address = "0",
                    Name = ADTSModel.Key,
                    Model = ADTSModel.Model,
                    DeviceCommonType = ADTSModel.DeviceCommonType,
                    DeviceManufacturer = ADTSModel.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(ADTSModel.TypesEtalonParameters),
                    SerialNumber = "123",
                    NamePort = "COM2"
                },
            };
            return res;
        }

        public List<DeviceTypeSettings> Devices { get; set; }

        public List<DeviceSettings> LastEtalons { get; set; }

        public List<DeviceSettings> LastDevices { get; set; }

        public List<ComPortSettings> Ports { get; set; }

    }
}
