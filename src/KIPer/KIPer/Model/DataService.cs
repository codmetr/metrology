using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using KIPer.Interfaces;
using KIPer.Settings;
using PACESeries;
using SQLiteArchive;

namespace KIPer.Model
{
    public class DataService : IDataService
    {
        MainLoop.ILoops _loops = new MainLoop.Loops();
        MineSettings _settings = new MineSettings();
        private string _settingsFileName = "settings";
        private PACE5000Model _pace5000;
        private IArchive _archive;


        public DataService(IArchive archive)
        {
            _archive = archive;
        }

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
            _settings = _archive.Load(_settingsFileName, MineSettings.GetDefault());
        }

        public void SaveSettings()
        {
            _archive.Save(_settingsFileName, _settings);
        }

        public PACE5000Model Pace5000
        {
            get { return _pace5000; }
        }
    }
}