using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using ADTS;
using ADTSChecks.Model.Devices;
using ArchiveData.DTO;
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

        readonly IMainSettings _settings;
        private readonly IArchive _archive;
        private IDeviceManager _deviceManager;
        private ResultsArchive _resultsArchive;
        private readonly List<IDeviceTypeDescriptor> _deviceTypes;
        private readonly List<IDeviceTypeDescriptor> _ethalonTypes;
        private readonly List<DeviceDescriptor> _etalons;


        public DataService(IArchive archive, IMainSettings settings)
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
            _deviceTypes.Add(new DeviceTypeDescriptor(ADTSModel.Model, ADTSModel.DeviceCommonType, ADTSModel.DeviceManufacturer));

            _ethalonTypes.Add(new DeviceTypeDescriptor(PACE1000Model.Model, PACE1000Model.DeviceCommonType, PACE1000Model.DeviceManufacturer));

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
        public IMainSettings Settings
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