using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIPer.Settings
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
                }
            };
            res.Etalons = new List<EtalonSettings>()
            {
                new EtalonSettings()
                {
                    Device = new DeviceSettings()
                    {
                        Address = "0",
                        Name = KIPer.Model.PACE5000Model.Key,
                        NamePort = "COM1"
                    },
                    Port = res.Ports.FirstOrDefault(el=>el.Name == "COM1")
                }
            };
            return res;
        }

        public List<EtalonSettings> Etalons;

        public List<ComPortSettings> Ports;


    }
}
