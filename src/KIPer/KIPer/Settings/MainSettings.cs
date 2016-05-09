using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Channels;
using KipTM.Model.Checks;

namespace KipTM.Settings
{
    public class MainSettings
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
                    Key = KipTM.Model.Devices.ADTSModel.Key,
                    Model = KipTM.Model.Devices.ADTSModel.Model,
                    DeviceCommonType = KipTM.Model.Devices.ADTSModel.DeviceCommonType,
                    DeviceManufacturer = KipTM.Model.Devices.ADTSModel.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(KipTM.Model.Devices.ADTSModel.TypesEtalonParameters),
                    //AvilableEthalonTypes = new List<string>(){KipTM.Model.Devices.PACE5000Model.Key, UserEchalonChannel.Key},
                },
                new DeviceTypeSettings()
                {
                    Key = KipTM.Model.Devices.PACE1000Model.Key,
                    Model = KipTM.Model.Devices.PACE1000Model.Model,
                    DeviceCommonType = KipTM.Model.Devices.PACE1000Model.DeviceCommonType,
                    DeviceManufacturer = KipTM.Model.Devices.PACE1000Model.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(KipTM.Model.Devices.ADTSModel.TypesEtalonParameters),
                    //AvilableEthalonTypes = new List<string>(){KipTM.Model.Devices.PACE5000Model.Key, UserEchalonChannel.Key},
                },

            };
            res.LastEtalons = new List<DeviceSettings>()
            {
                new DeviceSettings()
                {
                    Address = "0",
                    Name = KipTM.Model.Devices.PACE1000Model.Key,
                    Model = KipTM.Model.Devices.PACE1000Model.Model,
                    DeviceCommonType = KipTM.Model.Devices.PACE1000Model.DeviceCommonType,
                    DeviceManufacturer = KipTM.Model.Devices.PACE1000Model.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(KipTM.Model.Devices.PACE1000Model.TypesEtalonParameters),
                    SerialNumber = "123",
                    NamePort = "COM1"
                },
            };
            res.LastDevices = new List<DeviceSettings>()
            {
                new DeviceSettings()
                {
                    Address = "0",
                    Name = KipTM.Model.Devices.ADTSModel.Key,
                    Model = KipTM.Model.Devices.ADTSModel.Model,
                    DeviceCommonType = KipTM.Model.Devices.ADTSModel.DeviceCommonType,
                    DeviceManufacturer = KipTM.Model.Devices.ADTSModel.DeviceManufacturer,
                    TypesEtalonParameters = new List<string>(KipTM.Model.Devices.ADTSModel.TypesEtalonParameters),
                    SerialNumber = "123",
                    NamePort = "COM2"
                },
            };
            return res;
        }

        public List<DeviceTypeSettings> Devices;

        public List<DeviceSettings> LastEtalons;

        public List<DeviceSettings> LastDevices;

        public List<ComPortSettings> Ports;

    }
}
