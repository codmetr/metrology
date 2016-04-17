using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using ADTS;
using KipTM.Archive.DTO;
using KipTM.Interfaces;
using KipTM.Model.Checks;
using KipTM.Settings;
using KipTM.Model.Devices;
using PACESeries;
using SQLiteArchive;

namespace KipTM.Model
{
    public class DataService : IDataService, IDisposable
    {
        
        private const string ResultsArchiveFileName = "results";

        readonly MainSettings _settings;
        private readonly IArchive _archive;
        private IDeviceManager _deviceManager;
        private ResultsArchive _resultsArchive;
        private readonly List<IDeviceTypeDescriptor> _deviceTypes;
        private readonly List<IDeviceTypeDescriptor> _ethalonTypes;
        private readonly List<DeviceDescriptor> _etalons;


        public DataService(IArchive archive, MainSettings settings)
        {
            _archive = archive;
            _settings = settings;
            _deviceTypes = new List<IDeviceTypeDescriptor>();
            _ethalonTypes = new List<IDeviceTypeDescriptor>();
            _etalons = new List<DeviceDescriptor>();
            _resultsArchive = new ResultsArchive();
        }

        public void InitDevices()
        {
            DeviceSettings paceSettings = null;
            if (_settings.LastEtalons != null) 
                paceSettings = _settings.LastEtalons.FirstOrDefault(el => el.Name == PACE5000Model.Key);
            DeviceSettings adtsSettings = null;
            if (_settings.LastDevices != null)
                adtsSettings = _settings.LastDevices.FirstOrDefault(el => el.Name == ADTSModel.Key);

            if(paceSettings == null)
                throw new NullReferenceException(string.Format("PACE settings not found by key \"{0}\"", PACE5000Model.Key));
            if(adtsSettings == null)
                throw new NullReferenceException(string.Format("ADTS settings not found by key \"{0}\"", ADTSModel.Key));

            var adtsPort = _settings.Ports.FirstOrDefault(el => el.Name == adtsSettings.NamePort);
            if (adtsPort == null)
                throw new NullReferenceException(string.Format("ADTS port not found by key \"{0}\"", adtsSettings.NamePort));

            var pacePort = _settings.Ports.FirstOrDefault(el => el.Name == paceSettings.NamePort);
            if (pacePort == null)
                throw new NullReferenceException(string.Format("PACE port not found by key \"{0}\"", paceSettings.NamePort));

            _deviceTypes.Add(new DeviceTypeDescriptor(ADTSModel.Model, ADTSModel.DeviceCommonType, ADTSModel.DeviceManufacturer));

            _ethalonTypes.Add(new DeviceTypeDescriptor(PACE5000Model.Model, PACE5000Model.DeviceCommonType, PACE5000Model.DeviceManufacturer));

            _deviceManager = new DeviceManager(NLog.LogManager.GetLogger("DeviceManager"));
        }

        /// <summary>
        /// Пул сконфигурируемых устройств
        /// </summary>
        public IDeviceManager DeviceManager
        {
            get { return _deviceManager; }
        }

        /// <summary>
        /// Список типов поддерживаемых устройств
        /// </summary>
        public IEnumerable<IDeviceTypeDescriptor> DeviceTypes
        {
            get { return _deviceTypes; }
        }

        /// <summary>
        /// Список типов поддерживаемых эталонов
        /// </summary>
        public IEnumerable<IDeviceTypeDescriptor> EtalonTypes
        {
            get { return _ethalonTypes; }
        }

        /// <summary>
        /// Набор сконфигурированных эталонов
        /// </summary>
        public IEnumerable<DeviceDescriptor> Etalons
        {
            get { return _etalons; }
        }

        /// <summary>
        /// Архив результатов проверок
        /// </summary>
        public ResultsArchive ResultsArchive
        {
            get { return _resultsArchive; }
        }

        /// <summary>
        /// Настройки
        /// </summary>
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

        public void SaveSettings()
        {
            _archive.Save(MainSettings.SettingsFileName, _settings);
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