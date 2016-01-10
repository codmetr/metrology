using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using KIPer.Interfaces;
using KIPer.Settings;
using PACESeries;

namespace KIPer.Model
{
    public class DataService : IDataService
    {
        MainLoop.ILoops _loops = new MainLoop.Loops();
        MineSettings _settings = new MineSettings();
        private string _settingsPath = ".\\settings.xml";
        private PACE5000Model _pace5000;

        public void InitDevices()
        {
            var paceSettings = _settings.Etalons.FirstOrDefault(el => el.Device.Name == PACE5000Model.Key);
            if (paceSettings != null)
            {
                int address;
                if (int.TryParse(paceSettings.Device.Address, out address))
                    _pace5000 = new PACE5000Model("PACE 5000 - модульный контроллер давления/цифровой манометр", _loops, paceSettings.Port.Name, new PACEDriver(address));
            }
        }

        public void LoadSettings()
        {
            if(!File.Exists(_settingsPath))
            {
                _settings = MineSettings.GetDefault();
                return;
            }
            var xmlSerializer = new XmlSerializer(typeof (MineSettings));
            using (FileStream fs = new FileStream(_settingsPath, FileMode.Open))
            {
                XmlReader reader = XmlReader.Create(fs);

                _settings = (MineSettings) xmlSerializer.Deserialize(reader);
                fs.Close();
            }
        }

        public PACE5000Model Pace5000
        {
            get { return _pace5000; }
        }
    }
}