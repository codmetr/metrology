using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Checks;

namespace KipTM.Settings
{
    public class MainSettings
    {
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
            res.Etalons = new List<EtalonSettings>()
            {
                new EtalonSettings()
                {
                    Device = new DeviceSettings()
                    {
                        Address = "0",
                        Name = KipTM.Model.Devices.PACE5000Model.Key,
                        Model = KipTM.Model.Devices.PACE5000Model.Model,
                        DeviceCommonType = KipTM.Model.Devices.PACE5000Model.DeviceCommonType,
                        DeviceManufacturer = KipTM.Model.Devices.PACE5000Model.DeviceManufacturer,
                        TypesEtalonParameters = new List<string>(KipTM.Model.Devices.PACE5000Model.TypesEtalonParameters),
                        SerialNumber = "123",
                        NamePort = "COM1"
                    },
                    Port = res.Ports.FirstOrDefault(el=>el.Name == "COM1")
                }
            };
            res.Devices = new List<DeviceSettings>()
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
            res.Methodic = new List<MethodicSettings>()
            {
                new MethodicSettings()
                {
                    Name=ADTSCheckMethodic.KeySettingsPS,
                    Points = new List<PointTolerancePair>()
                    {
                        new PointTolerancePair(){Point = "27.62",   Tolerance = "0.1"},
                        new PointTolerancePair(){Point = "72.00",   Tolerance = "0.1"},
                        new PointTolerancePair(){Point = "189.00",  Tolerance = "0.1"},
                        new PointTolerancePair(){Point = "466.00",  Tolerance = "0.1"},
                        new PointTolerancePair(){Point = "697.00",  Tolerance = "0.1"},
                        new PointTolerancePair(){Point = "843.00",  Tolerance = "0.1"},
                        new PointTolerancePair(){Point = "1013.00", Tolerance = "0.1"},
                        new PointTolerancePair(){Point = "1100.00", Tolerance = "0.1"},
                    }
                },
                new MethodicSettings()
                {
                    Name=ADTSCheckMethodic.KeySettingsPT,
                    Points = new List<PointTolerancePair>()
                    {
                        new PointTolerancePair(){Point = "27.62",   Tolerance = "0.24"},
                        new PointTolerancePair(){Point = "72.00",   Tolerance = "0.24"},
                        new PointTolerancePair(){Point = "189.00",  Tolerance = "0.25"},
                        new PointTolerancePair(){Point = "466.00",  Tolerance = "0.25"},
                        new PointTolerancePair(){Point = "697.00",  Tolerance = "0.26"},
                        new PointTolerancePair(){Point = "843.00",  Tolerance = "0.27"},
                        new PointTolerancePair(){Point = "1013.00", Tolerance = "0.27"},
                        new PointTolerancePair(){Point = "1100.00", Tolerance = "0.28"},
                        new PointTolerancePair(){Point = "1655.00", Tolerance = "0.32"},
                        new PointTolerancePair(){Point = "2200.00", Tolerance = "0.36"},
                        new PointTolerancePair(){Point = "2590.00", Tolerance = "0.40"},
                        new PointTolerancePair(){Point = "3000.00", Tolerance = "0.46"},
                        new PointTolerancePair(){Point = "3500.00", Tolerance = "0.49"},
                    }
                },
            };
            return res;
        }

        public List<EtalonSettings> Etalons;

        public List<DeviceSettings> Devices;

        public List<ComPortSettings> Ports;

        public List<MethodicSettings> Methodic;

    }
}
