using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using ADTS;
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
        private readonly Dictionary<string, ICheckMethodic> _methodics;
        private ResultsArchive _resultsArchive;
        private readonly List<IDeviceTypeDescriptor> _deviceTypes;
        private readonly List<IDeviceTypeDescriptor> _ethalonTypes;
        private readonly List<DeviceDescriptor> _etalons;


        public DataService(IArchive archive)
        {
            _archive = archive;
            _deviceTypes = new List<IDeviceTypeDescriptor>();
            _ethalonTypes = new List<IDeviceTypeDescriptor>();
            _etalons = new List<DeviceDescriptor>();
            _methodics = new Dictionary<string, ICheckMethodic>();
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

            _deviceTypes.Add(new DeviceTypeDescriptor(ADTSModel.Model, ADTSModel.DeviceCommonType, ADTSModel.DeviceManufacturer));

            _ethalonTypes.Add(new DeviceTypeDescriptor(PACE5000Model.Model, PACE5000Model.DeviceCommonType, PACE5000Model.DeviceManufacturer));

            _deviceManager = new DeviceManager(adtsPort, paceSettings.Port, adtsSettings, paceSettings.Device, NLog.LogManager.GetLogger("DeviceManager"));

            var adtsCheck = new ADTSCheckMethodic(_deviceManager.ADTS, NLog.LogManager.GetLogger("ADTSCheckMethodic"));
            adtsCheck.Init(new ADTSCheckParameters(CalibChannel.PS,
                _settings.Methodic.First(el => el.Name == ADTSCheckMethodic.KeySettingsPS)
                    .Points.ToDictionary(
                        elK => double.Parse(elK.Point, NumberStyles.Any, CultureInfo.InvariantCulture),
                        elK => double.Parse(elK.Tolerance, NumberStyles.Any, CultureInfo.InvariantCulture)),
                        null, //todo: придумать способ запроса состояний у пользователя
                        null)); //todo: придумать способ запроса решения о применении у пользователя
            _methodics.Add(ADTSModel.Key, adtsCheck);
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
        /// Набор поддерживаемых методик
        /// </summary>
        public IDictionary<string, ICheckMethodic> Methodics
        {
            get { return _methodics; }
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

        public void LoadSettings()
        {
            _settings = _archive.Load(SettingsFileName, MainSettings.GetDefault());

            // TODO Загружать настройки эталона из настроек
            _etalons.Add(new DeviceDescriptor(new DeviceTypeDescriptor(PACE5000Model.Model, PACE5000Model.DeviceCommonType, PACE5000Model.DeviceManufacturer), "123"));
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