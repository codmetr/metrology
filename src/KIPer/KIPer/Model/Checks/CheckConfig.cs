using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using KipTM.Archive;
using KipTM.Archive.DTO;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;
using KipTM.Settings;

namespace KipTM.Model.Checks
{
    public class CheckConfig
    {
        #region Внутренние переменные
        private readonly IMethodsService _methodics;
        private IDictionary<string, ICheckMethod> _check;
        private readonly IPropertyPool _propertyPool;
        private string _selectedCheckTypeKey;
        private ICheckMethod _selectedCheckType;
        private TestResult _result;

        private IDictionary<string, DeviceTypeDescriptor> _avalableDeviceTypes;
        private string _devTypeKey;
        private DeviceTypeDescriptor _selectedType;

        private IDictionary<string, DeviceTypeDescriptor> _avalableEthalonTypes;
        private string _ethalonTypeKey;
        private DeviceTypeDescriptor _selectedEthalonType;

        private DeviceDescriptor _ethalon;
        private bool _isAnalogEthalon;
        private IEnumerable<string> _channels;

        #endregion

        #region Конструкторы и инициализация
        public CheckConfig(MainSettings settings, IMethodsService methodics, IPropertyPool propertyPool, DictionariesPool dictionaries, TestResult result)
        {
            _result = result;
            _methodics = methodics;
            _propertyPool = propertyPool;

            LoadAvalableCheckDevices(settings, dictionaries);

            LoadAvalableEthalons(settings);
        }

        /// <summary>
        /// Загрузить набор доступных проверяемых устройств
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="dictionaries"></param>
        private void LoadAvalableCheckDevices(MainSettings settings, DictionariesPool dictionaries)
        {
            // заполнение списка поддерживаемых устройств и выбор первого элемента
            var avalableDeviceTypes = new Dictionary<string, DeviceTypeDescriptor>();
            _devTypeKey = null;
            foreach (var deviceType in dictionaries.DeviceTypes)
            {
                var setDevice = settings.Devices.First(el => el.Key == deviceType);
                if (setDevice == null)
                    continue;
                if (_devTypeKey == null)
                    _devTypeKey = deviceType;
                avalableDeviceTypes.Add(deviceType,
                    new DeviceTypeDescriptor(setDevice.Model, setDevice.DeviceCommonType, setDevice.DeviceManufacturer));
            }
            _avalableDeviceTypes = avalableDeviceTypes;
            _selectedCheckTypeKey = DeviceTypes.First();
            _selectedType = avalableDeviceTypes[_selectedCheckTypeKey];

            if (_devTypeKey != null)
            {
                _result.TargetDevice = new DeviceDescriptor(_selectedType);
                _check = _methodics.MethodsForType(_devTypeKey);
                Channels = _propertyPool.ByKey(_devTypeKey).GetAllKeys();
                _result.Channel = Channels.First();
                SelectedCheckTypeKey = CheckTypes.First();
            }
        }

        /// <summary>
        /// Загрузить набор допустимых эталонов
        /// </summary>
        /// <param name="settings"></param>
        private void LoadAvalableEthalons(MainSettings settings)
        {
            var avalableEthalonTypes = new Dictionary<string, DeviceTypeDescriptor>();
            _ethalonTypeKey = null;
            var ethalons = _propertyPool.ByKey(_devTypeKey).ByKey(SelectedChannel).GetProperty<List<string>>(CommonPropertyKeys.KeyEthalons);
            foreach (var deviceEthalon in ethalons)
            {
                var setDevice = settings.Devices.FirstOrDefault(el => el.Key == deviceEthalon);
                if (_ethalonTypeKey == null)
                {
                    _ethalonTypeKey = deviceEthalon;
                }
                if (setDevice == null)
                {
                    if (_ethalonTypeKey == UserEthalonChannel.Key)
                        avalableEthalonTypes.Add(deviceEthalon, 
                            new DeviceTypeDescriptor("Аналоговый прибор", "Приборы без аппаратного интерфейса", ""));
                    else
                        avalableEthalonTypes.Add(deviceEthalon, null);
                    continue;
                }
                avalableEthalonTypes.Add(deviceEthalon,
                    new DeviceTypeDescriptor(setDevice.Model, setDevice.DeviceCommonType, setDevice.DeviceManufacturer));
            }
            _avalableEthalonTypes = avalableEthalonTypes;
            SelectedEthalonTypeKey = EthalonTypes.First();

            var ethalonDevType = new DeviceTypeDescriptor("", "", "");
            if (_devTypeKey != null)
            {
                if(_ethalonTypeKey != UserEthalonChannel.Key)
                    ethalonDevType = SelectedEthalonType;
                Ethalon = new DeviceDescriptor(ethalonDevType);
                _result.Etalon = new List<DeviceDescriptor>() { Ethalon };
            }
        }
        #endregion

        #region Перечисления
        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IEnumerable<string> DeviceTypes { get { return _avalableDeviceTypes.Keys; } }

        /// <summary>
        /// Каналы устройства
        /// </summary>
        public IEnumerable<string> Channels
        {
            get { return _channels; }
            set { _channels = value; }
        }

        /// <summary>
        /// Дострупные для выбранного типа устройства методики
        /// </summary>
        public IEnumerable<string> CheckTypes
        {
            get { return _check.Keys; }
        }

        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IEnumerable<string> EthalonTypes { get { return _avalableEthalonTypes.Keys; } }

        #endregion

        #region Контейнер результата
        /// <summary>
        /// Контейнер результата
        /// </summary>
        public TestResult Result
        {
            get { return _result; }
            protected set { _result = value; }
        }
        #endregion

        #region Условия проверки
        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime CheckDateTime { get { return _result.Timestamp; } set { _result.Timestamp = value; } }

        /// <summary>
        /// Лаборатория
        /// </summary>
        public string Laboratory { get { return _result.Laboratory; } set { _result.Laboratory = value; } }

        /// <summary>
        /// Температура
        /// </summary>
        public string Temperature { get { return _result.Temperature; } set { _result.Temperature = value; } }

        /// <summary>
        /// Влажность
        /// </summary>
        public string Humidity { get { return _result.Humidity; } set { _result.Humidity = value; } }
        #endregion

        #region Настройки проверяемого устройства
        /// <summary>
        /// Тип устройства
        /// </summary>
        public DeviceTypeDescriptor SelectedDeviceType
        {
            get { return _selectedType; }
            private set
            {
                if (_selectedType == value)
                    return;
                _selectedType = value;
                _result.TargetDevice.DeviceType = _selectedType;
            }
        }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public string SelectedDeviceTypeKey
        {
            get { return _devTypeKey; }
            set
            {
                if (_devTypeKey == value)
                    return;
                _devTypeKey = value;
                SelectedDeviceType = _avalableDeviceTypes[_devTypeKey];
                _result.TargetDevice.DeviceType = SelectedDeviceType;
                _check = _methodics.MethodsForType(_devTypeKey);
            }
        }

        /// <summary>
        /// Производитель
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
        public string SelectedCheckTypeKey
        {
            get { return _selectedCheckTypeKey; }
            set
            {
                _selectedCheckTypeKey = value;
                _result.CheckType = _selectedCheckTypeKey;
                SelectedCheckType = _check[_selectedCheckTypeKey];
                var properties = _propertyPool.ByKey(_devTypeKey).ByKey(SelectedChannel);
                SelectedCheckType.Init(properties);
            }
        }

        /// <summary>
        /// Выбранная методика
        /// </summary>
        public ICheckMethod SelectedCheckType
        {
            get { return _selectedCheckType; }
            private set
            {
                _selectedCheckType = value;
                OnSelectedCheckTypeChanged();
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
                if (value == _result.Channel)
                    return;
                _result.Channel = value;
                var properties = _propertyPool.ByKey(_devTypeKey).ByKey(value);
                SelectedCheckType.Init(properties); //todo maybe move to CheckViewModel
                OnSelectedChannelChanged();
            }
        }
        #endregion

        #region Настройки эталона

        /// <summary>
        /// Тип устройства
        /// </summary>
        public string SelectedEthalonTypeKey
        {
            get { return _ethalonTypeKey; }
            set
            {
                _ethalonTypeKey = value;
                IsAnalogEthalon = _ethalonTypeKey == UserEthalonChannel.Key;
                SelectedEthalonType = _avalableEthalonTypes[_ethalonTypeKey];
            }
        }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public DeviceTypeDescriptor SelectedEthalonType
        {
            get { return _selectedEthalonType; }
            set
            {
               _selectedEthalonType = value;
                if (!IsAnalogEthalon)
                    Ethalon.DeviceType = _selectedEthalonType;
                OnSelectedEthalonTypeChanged();
            }
        }

        public bool IsAnalogEthalon
        {
            get { return _isAnalogEthalon; }
            set
            {
                _isAnalogEthalon = value;
            }
        }

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string EthalonDeviceType
        {
            get { return Ethalon.DeviceType.Model; }
            set
            {
                if (!IsAnalogEthalon)
                    throw new SettingsPropertyIsReadOnlyException("EthalonDeviceType can not set in no user channel");
                Ethalon.DeviceType = new DeviceTypeDescriptor(value, Ethalon.DeviceType.DeviceCommonType, Ethalon.DeviceType.DeviceManufacturer);
            }
        }

        /// <summary>
        /// Производитель эталона
        /// </summary>
        public string EthalonManufacturer
        {
            get { return Ethalon.DeviceType.DeviceManufacturer; }
            set
            {
                if (!IsAnalogEthalon)
                    throw new SettingsPropertyIsReadOnlyException("EthalonManufacturer can not set in no user channel");
                Ethalon.DeviceType = new DeviceTypeDescriptor(Ethalon.DeviceType.Model, _ethalon.DeviceType.DeviceCommonType, value);
            }
        }

        /// <summary>
        /// Инвентарный номер эталона
        /// </summary>
        public string EthalonInventarNumber
        {
            get { return Ethalon.InventarNumber; }
            set { Ethalon.InventarNumber = value; }
        }

        /// <summary>
        /// Серийный номер эталона
        /// </summary>
        public string EthalonSerialNumber
        {
            get { return Ethalon.SerialNumber; }
            set { Ethalon.SerialNumber = value; }
        }

        /// <summary>
        /// Дата предыдущей поверки/калибровки эталона
        /// </summary>
        public DateTime EthalonPreviousCheckTime
        {
            get { return Ethalon.PreviousCheckTime; }
            set { Ethalon.PreviousCheckTime = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DeviceDescriptor Ethalon
        {
            get { return _ethalon; }
            protected set { _ethalon = value; }
        }

        #endregion

        #region Events

        public event EventHandler SelectedCheckTypeChanged;

        protected virtual void OnSelectedCheckTypeChanged()
        {
            EventHandler handler = SelectedCheckTypeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler SelectedChannelChanged;

        protected virtual void OnSelectedChannelChanged()
        {
            EventHandler handler = SelectedChannelChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler SelectedEthalonTypeChanged;

        protected virtual void OnSelectedEthalonTypeChanged()
        {
            EventHandler handler = SelectedEthalonTypeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
