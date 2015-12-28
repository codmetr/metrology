using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using KIPer.Settings;

namespace KIPer.Model
{
    public class DataService : IDataService
    {
        MainLoop.ILoops _loops = new MainLoop.Loops();
        MineSettings _settings = new MineSettings();
        private string _settingsPath = ".\\settings.xml";

        public void GetData(Action<PACE5000Model, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new PACE5000Model("Welcome to MVVM Light");
            callback(item, null);
        }

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
    }
}