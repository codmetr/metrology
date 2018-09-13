using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using ArchiveData.DTO;
using CheckFrame.Model;
using KipTM.Interfaces;
using KipTM.Interfaces.Checks;
using KipTM.Model.Channels;
using KipTM.Settings;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using PACESeries;
using SQLiteArchive;

namespace KipTM.Model
{
    /// <summary>
    /// Сервис хранимых данных (настройки и результаты проверки)
    /// </summary>
    public class DataService : IDataService, IDisposable
    {
        
        private const string ResultsArchiveFileName = "results";

        readonly IMainSettings _settings;
        private readonly IArchive _archive;
        //private IDeviceManager _deviceManager;
        private ResultsArchive _resultsArchive;
        /// <summary>
        /// Список типов поддерживаемых устройств
        /// </summary>
        private readonly List<IDeviceTypeDescriptor> _deviceTypes;
        /// <summary>
        /// Список типов поддерживаемых эталонов
        /// </summary>
        private readonly List<IDeviceTypeDescriptor> _etalonTypes;
        /// <summary>
        /// Набор сконфигурированных эталонов
        /// </summary>
        private readonly List<DeviceDescriptor> _etalons;


        public DataService(IArchive archive, IMainSettings settings)
        {
            _archive = archive;
            _settings = settings;
            _deviceTypes = new List<IDeviceTypeDescriptor>();
            _etalonTypes = new List<IDeviceTypeDescriptor>();
            _etalons = new List<DeviceDescriptor>();
            _resultsArchive = new ResultsArchive();
        }

        public void FillDeviceList(IEnumerable<DeviceTypeDescriptor> deviceTypes, IEnumerable<DeviceTypeDescriptor> etalonTypes)
        {
            _deviceTypes.AddRange(deviceTypes);
            _etalonTypes.AddRange(etalonTypes);
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
            get { return _etalonTypes; }
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

        #endregion

        public void Dispose()
        {

        }
    }
}