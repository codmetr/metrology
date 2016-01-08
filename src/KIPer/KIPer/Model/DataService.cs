using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using KIPer.Interfaces;
using KIPer.Settings;

namespace KIPer.Model
{
    public class DataService : IDataService
    {
        MainLoop.ILoops _loops = new MainLoop.Loops();
        MineSettings _settings = new MineSettings();
        private string _settingsPath = ".\\settings.xml";
        private PACE5000Model _pace5000 = new PACE5000Model("PACE5000");

        public void InitDevices()
        {
            
        }

        public void LoadSettings()
        {
            if(!File.Exists(_settingsPath))
                return;
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
            set { _pace5000 = value; }
        }
    }
}