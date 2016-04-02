using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using ADTS;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KipTM.Archive;
using KipTM.Archive.DTO;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Settings;
using KipTM.ViewModel.Checks;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CheckViewModel : ViewModelBase
    {
        private readonly IDeviceManager _deviceManager;
        private DictionariesPool _dictionaries;
        private MainSettings _settings;
        private readonly IMethodsService _methodics;
        private IDictionary<string, ICheckMethod> _check;
        private readonly IPropertyPool _propertyPool;
        private object _selectedCheck;
        private KeyValuePair<string, ICheckMethod> _selectedCheckType;
        private TestResult _result;
        private IDictionary<string, DeviceTypeDescriptor> _avalableDeviceTypes; 
        private string _devTypeKey;
        private KeyValuePair<string, DeviceTypeDescriptor> _selectedType;

        private IDictionary<string, DeviceTypeDescriptor> _avalableEthalonTypes; 
        private string _ethalonTypeKey;
        private KeyValuePair<string, DeviceTypeDescriptor> _selectedEthalonType;

        private DeviceDescriptor _ethalon;
        private bool _isAnalogEthalon;

        /// <summary>
        /// For disiner
        /// </summary>
        public CheckViewModel()
        {}
        /// <summary>
        /// Initializes a new instance of the CheckViewModel class.
        /// </summary>
        public CheckViewModel(MainSettings settings, IMethodsService methodics, IPropertyPool propertyPool, DictionariesPool dictionaries, IDeviceManager deviceManager)
        {
            _result = new TestResult();
            _settings = settings;
            _methodics = methodics;
            _propertyPool = propertyPool;
            _dictionaries = dictionaries;
            _deviceManager = deviceManager;

            LoadAvalableCheckDevices();

            LoadAvalableEthalons();
        }

        private void LoadAvalableCheckDevices()
        {
            // заполнение списка поддерживаемых устройств и выбор первого элемента
            var avalableDeviceTypes = new Dictionary<string, DeviceTypeDescriptor>();
            _devTypeKey = null;
            foreach (var deviceType in _dictionaries.DeviceTypes)
            {
                var setDevice = _settings.Devices.First(el => el.Key == deviceType);
                if (setDevice == null)
                    continue;
                if (_devTypeKey == null)
                    _devTypeKey = deviceType;
                avalableDeviceTypes.Add(deviceType,
                    new DeviceTypeDescriptor(setDevice.Model, setDevice.DeviceCommonType, setDevice.DeviceManufacturer));
            }
            _avalableDeviceTypes = avalableDeviceTypes;
            _selectedType = _avalableDeviceTypes.First();

            if (_devTypeKey != null)
            {
                _result.TargetDevice = new DeviceDescriptor(_selectedType.Value);
                _check = _methodics.MethodsForType(_devTypeKey);
                SelectedCheckType = _check.First();
                Channels = _propertyPool.ByKey(_devTypeKey).GetAllKeys();
                _result.Channel = Channels.First();
            }
        }

        private void LoadAvalableEthalons()
        {
            var avalableEthalonTypes = new Dictionary<string, DeviceTypeDescriptor>();
            _ethalonTypeKey = null;
            var ethalons = _propertyPool.ByKey(_devTypeKey).ByKey(SelectedChannel).GetProperty<List<string>>(CommonPropertyKeys.KeyEthalons);
            foreach (var deviceEthalon in ethalons)
            {
                var setDevice = _settings.Devices.FirstOrDefault(el => el.Key == deviceEthalon);
                if (_ethalonTypeKey == null)
                {
                    _ethalonTypeKey = deviceEthalon;
                    if (_ethalonTypeKey == UserEchalonChannel.Key)
                        IsAnalogEthalon = true;
                }
                if (setDevice == null)
                {
                    avalableEthalonTypes.Add(deviceEthalon, null);
                    continue;
                }
                avalableEthalonTypes.Add(deviceEthalon,
                    new DeviceTypeDescriptor(setDevice.Model, setDevice.DeviceCommonType, setDevice.DeviceManufacturer));
            }
            _avalableEthalonTypes = avalableEthalonTypes;
            _selectedEthalonType = _avalableEthalonTypes.First();

            var ethalonDevType = new DeviceTypeDescriptor("", "", "");
            if (_devTypeKey != null)
            {
                if(_ethalonTypeKey != UserEchalonChannel.Key)
                    ethalonDevType = _selectedEthalonType.Value;
                Ethalon = new DeviceDescriptor(ethalonDevType);
                _result.Etalon = new List<DeviceDescriptor>() { _ethalon };
            }
        }

        #region Перечисления
        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IDictionary<string, DeviceTypeDescriptor> DeviceTypes { get { return _avalableDeviceTypes; } }

        /// <summary>
        /// Каналы устройства
        /// </summary>
        public IEnumerable<string> Channels { get; set; }

        /// <summary>
        /// Дострупные для выбранного типа устройства методики
        /// </summary>
        public IDictionary<string, ICheckMethod> CheckTypes
        {
            get { return _check; }
            set { Set(ref _check, value); }
        }

        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IDictionary<string, DeviceTypeDescriptor> EthalonTypes { get { return _avalableEthalonTypes; } }

        #endregion

        #region Фактические настройки
        
        #region Условия проверки
        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime CheckDateTime { get; set; }

        /// <summary>
        /// Лаборатория
        /// </summary>
        public string Laboratory { get; set; }

        /// <summary>
        /// Температура
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        public string Humidity { get; set; }
        #endregion

        #region Настройки проверяемого устройства
        /// <summary>
        /// Тип устройства
        /// </summary>
        public KeyValuePair<string, DeviceTypeDescriptor> SelectedDeviceType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType.Key == value.Key)
                    return;
                _devTypeKey = _selectedType.Key;
                _result.TargetDevice.DeviceType = _selectedType.Value;
                CheckTypes = _methodics.MethodsForType(_selectedType.Key);
                RaisePropertyChanged("Manufacturer");
            }
        }

        public TestResult Result
        {
            get { return _result; }
            set { Set(ref _result, value); }
        }
        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string Manufacturer { get { return _result.TargetDevice.DeviceType.DeviceManufacturer; } }

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string InventarNumber
        {
            get { return _result.TargetDevice.InventarNumber; }
            set { _result.TargetDevice.InventarNumber = value; }
        }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber
        {
            get { return _result.TargetDevice.SerialNumber; }
            set { _result.TargetDevice.SerialNumber = value; }
        }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime PreviousCheckTime
        {
            get { return _result.TargetDevice.PreviousCheckTime; }
            set { _result.TargetDevice.PreviousCheckTime = value; }
        }

        /// <summary>
        /// Выбранная методика
        /// </summary>
        public KeyValuePair<string, ICheckMethod> SelectedCheckType
        {
            get { return _selectedCheckType; }
            set
            {
                _selectedCheckType = value;
                _result.CheckType = _selectedCheckType.Key;
                Check = GetViewModelFor(_selectedCheckType.Value);
            }
        }
 
        /// <summary>
        /// Выбранный канал
        /// </summary>
        public string SelectedChannel
        {
            get { return _result.Channel; }
            set
            {
                _result.Channel = value;
                var properties = _propertyPool.ByKey(_devTypeKey).ByKey(value);
                _selectedCheckType.Value.Init(properties);
            }
        }

        /// <summary>
        /// Выбранный порт
        /// </summary>
        public string SelectedPort { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        #endregion

        #region Настройки эталона

        /// <summary>
        /// Тип устройства
        /// </summary>
        public KeyValuePair<string, DeviceTypeDescriptor> SelectedEthalonType
        {
            get { return _selectedEthalonType; }
            set
            {
                Set(ref _selectedEthalonType, value);
                IsAnalogEthalon = _selectedEthalonType.Key == UserEchalonChannel.Key;
                if (!IsAnalogEthalon)
                {
                    Ethalon.DeviceType = _selectedEthalonType.Value;
                    return;
                }
            }
        }

        public bool IsAnalogEthalon
        {
            get { return _isAnalogEthalon; }
            set
            {
                Set(ref _isAnalogEthalon, value); 
                RaisePropertyChanged("IsNoAnalogEthalon");
            }
        }

        public bool IsNoAnalogEthalon
        {
            get { return !_isAnalogEthalon; }
        }

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string EthalonDeviceType
        {
            get { return Ethalon.DeviceType.Model; }
            set
            {
                if(!IsAnalogEthalon)
                    throw new SettingsPropertyIsReadOnlyException("EthalonDeviceType can not set in no user channel");
                Ethalon.DeviceType = new DeviceTypeDescriptor(value, Ethalon.DeviceType.DeviceCommonType, Ethalon.DeviceType.DeviceManufacturer);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string EthalonManufacturer
        {
            get { return Ethalon.DeviceType.DeviceManufacturer; }
            set
            {
                if(!IsAnalogEthalon)
                    throw new SettingsPropertyIsReadOnlyException("EthalonManufacturer can not set in no user channel");
                Ethalon.DeviceType = new DeviceTypeDescriptor(Ethalon.DeviceType.Model, _ethalon.DeviceType.DeviceCommonType, value);
            }
        }

        public DeviceDescriptor Ethalon
        {
            get { return _ethalon; }
            protected set { Set(ref _ethalon, value); }
        }
        #endregion
        #endregion

        public ICommand Save { get; set; }

        public ICommand Report { get; set; }

        public object Check
        {
            get { return _selectedCheck; }
            set { Set(ref _selectedCheck, value); }
        }

        public object GetViewModelFor(ICheckMethod methodic)
        {
            if (methodic is ADTSCheckMethod)
            {
                var adtsMethodic = methodic as ADTSCheckMethod;
                adtsMethodic.SetADTS(_deviceManager.ADTS);
                return new ADTSCalibrationViewModel(adtsMethodic, _propertyPool.ByKey(_devTypeKey));
            }
            return null;
        }
    }
}