using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Settings
{
    public class MineSettings
    {
        public static MineSettings GetDefault()
        {
            var res = new MineSettings();
            res.Ports = new List<ComPortSettings>()
            {
                new ComPortSettings()
                {
                    Name = "COM1",
                    NumberCom = 1,
                    Rate = 9600,
                    Parity = Parity.None,
                    CountBits = 8,
                    CountStopBits = 1
                },
                new ComPortSettings()
                {
                    Name = "COM2",
                    NumberCom = 1,
                    Rate = 9600,
                    Parity = Parity.None,
                    CountBits = 8,
                    CountStopBits = 1
                }
            };
            res.Etalons = new List<EtalonSettings>()
            {
                new EtalonSettings()
                {
                    Device = new DeviceSettings()
                    {
                        Address = "0",
                        Name = KipTM.Model.PACE5000Model.Key,
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
                    Name = KipTM.Model.ADTSModel.Key,
                    NamePort = "COM2"
                },
            };
            return res;
        }

        public List<EtalonSettings> Etalons;

        public List<DeviceSettings> Devices;

        public List<ComPortSettings> Ports;


    }
}
