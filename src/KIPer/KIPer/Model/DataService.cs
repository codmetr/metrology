using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using KipTM.Interfaces;
using KipTM.Model.Archive;
using KipTM.Model.Checks;
using KipTM.Settings;
using KipTM.Model.Devices;
using PACESeries;
using SQLiteArchive;

namespace KipTM.Model
{
    public class DataService : IDataService, IDisposable
    {
        private const string SettingsFileName = "settings";
        private const string ResultsArchiveFileName = "results";

        MainSettings _settings = new MainSettings();
        private readonly IArchive _archive;
        private IDeviceManager _deviceManager;
        private readonly List<ICheckMethodic> _methodics;
        private ResultsArchive _resultsArchive;


        public DataService(IArchive archive)
        {
            _archive = archive;
            _methodics = new List<ICheckMethodic>();
            _resultsArchive = new ResultsArchive();
        }

        public void InitDevices()
        {
            var paceSettings = _settings.Etalons.FirstOrDefault(el => el.Device.Name == PACE5000Model.Key);
            var adtsSettings = _settings.Devices.FirstOrDefault(el => el.Name == ADTSModel.Key);

            if(paceSettings == null)
                throw new NullReferenceException(string.Format("PACE settings not found by key \"{0}\"", PACE5000Model.Key));
            if(adtsSettings == null)
                throw new NullReferenceException(string.Format("ADTS settings not found by key \"{0}\"", ADTSModel.Key));

            var adtsPort = _settings.Ports.FirstOrDefault(el => el.Name == adtsSettings.NamePort);
            if (adtsPort == null)
                throw new NullReferenceException(string.Format("ADTS port not found by key \"{0}\"", adtsSettings.NamePort));

            _deviceManager = new DeviceManager(adtsPort, paceSettings.Port, adtsSettings, paceSettings.Device, NLog.LogManager.GetCurrentClassLogger(typeof(DeviceManager)));
            _methodics.Add(new ADTSCheckMethodic(_deviceManager.ADTS, NLog.LogManager.GetCurrentClassLogger(typeof(ADTSCheckMethodic))));
        }

        public IDeviceManager DeviceManager
        {
            get { return _deviceManager; }
        }

        public IEnumerable<ICheckMethodic> Methodics
        {
            get { return _methodics; }
        }

        public ResultsArchive ResultsArchive
        {
            get { return _resultsArchive; }
        }

        public MainSettings Settings
        {
            get { return _settings; }
        }

        #region Load/Save
        public void LoadResults()
        {
            _resultsArchive = _archive.Load(ResultsArchiveFileName, _resultsArchive);
        }

        public void SaveResults()
        {
            _archive.Save(ResultsArchiveFileName, _resultsArchive);
        }

        public void LoadSettings()
        {
            _settings = _archive.Load(SettingsFileName, MainSettings.GetDefault());
        }

        public void SaveSettings()
        {
            _archive.Save(SettingsFileName, _settings);
        }
        #endregion

        public void Dispose()
        {
            var disp = _deviceManager as IDisposable;
            if (disp != null) 
                disp.Dispose();
        }
    }
}