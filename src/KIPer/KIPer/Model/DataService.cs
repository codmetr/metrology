using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using KipTM.Interfaces;
using KipTM.Settings;
using KipTM.Model.Devices;
using PACESeries;
using SQLiteArchive;

namespace KipTM.Model
{
    public class DataService : IDataService
    {
        MainLoop.ILoops _loops = new MainLoop.Loops();
        MineSettings _settings = new MineSettings();
        private string _settingsFileName = "settings";
        private IArchive _archive;
        private DeviceManager _deviceManager;


        public DataService(IArchive archive)
        {
            _archive = archive;
        }

        public void InitDevices()
        {
            var paceSettings = _settings.Etalons.FirstOrDefault(el => el.Device.Name == PACE5000Model.Key);
            var adtsSettings = _settings.Etalons.FirstOrDefault(el => el.Device.Name == ADTSModel.Key);

            if(paceSettings == null)
                throw new NullReferenceException(string.Format("PACE settings not found by key \"{0}\"", PACE5000Model.Key));
            if(adtsSettings == null)
                throw new NullReferenceException(string.Format("ADTS settings not found by key \"{0}\"", ADTSModel.Key));

            _deviceManager = new DeviceManager(adtsSettings.Port, paceSettings.Port, adtsSettings.Device, paceSettings.Device);
        }

        public IDeviceManager DeviceManager
        {
            get { return _deviceManager; }
        }

        public void LoadSettings()
        {
            _settings = _archive.Load(_settingsFileName, MineSettings.GetDefault());
        }

        public void SaveSettings()
        {
            _archive.Save(_settingsFileName, _settings);
        }
    }
}