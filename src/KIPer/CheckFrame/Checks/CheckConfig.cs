using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ArchiveData.DTO;
using CheckFrame.Archive;
using CheckFrame.Checks;
using CheckFrame.Model.Channels;
using CheckFrame.Model.Checks;
using KipTM.Archive;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.Settings;

namespace KipTM.Checks
{
    public class CheckConfig
    {
        #region Внутренние переменные

        private readonly IMethodsService _method;
        private IDictionary<string, ICheckMethod> _checks;
        private readonly IPropertyPool _propertyPool;
        private ICheckMethod _selectedCheckType;

        /// <summary>
        /// Доступные типы объектов конроля и их описатели
        /// </summary>
        private IDictionary<string, DeviceTypeDescriptor> _avalableDeviceTypes;
        /// <summary>
        /// Дступные типы эталонов
        /// </summary>
        private IDictionary<string, DeviceTypeDescriptor> _avalableEthalonTypes;
        /// <summary>
        /// Доступные измерительные каналы
        /// </summary>
        private IEnumerable<ChannelDescriptor> _channels;
        /// <summary>
        /// Ключи измерительных каналов
        /// </summary>
        /// <remarks>
        /// Необходимы для получения описателя по ключу выбираемому пользователем
        /// </remarks>
        private IDictionary<ChannelDescriptor, string> _channelKeys;

        /// <summary>
        /// Контейнер с результатом
        /// </summary>
        private TestResult _result;
        /// <summary>
        /// Конфигурация
        /// </summary>
        private CheckConfigData _data;

        #endregion

        #region Данные наcтроек
        /// <summary>
        /// Данные настроек
        /// </summary>
        public CheckConfigData Data
        {
            get { return _data; }
        }

        #endregion

        #region Конструкторы и инициализация
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="method"></param>
        /// <param name="propertyPool"></param>
        /// <param name="dictionaries"></param>
        /// <param name="result"></param>
        public CheckConfig(IMainSettings settings, IMethodsService method, IPropertyPool propertyPool, DictionariesPool dictionaries, TestResult result)
        {
            _data = new CheckConfigData();
            _result = result;
            _method = method;
            _propertyPool = propertyPool;

            LoadAvalableCheckDevices(settings, dictionaries);
            LoadAvalableEthalons(settings);
        }

        /// <summary>
        /// Загрузить набор доступных проверяемых устройств
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="dictionaries"></param>
        private void LoadAvalableCheckDevices(IMainSettings settings, DictionariesPool dictionaries)
        {
            // заполнение списка поддерживаемых устройств и выбор первого элемента
            var avalableDeviceTypes = new Dictionary<string, DeviceTypeDescriptor>();
            _data.TargetTypeKey = null;
            foreach (var deviceType in dictionaries.DeviceTypes)
            {
                var setDevice = settings.Devices.First(el => el.Key == deviceType);
                if (setDevice == null)
                    continue;
                if (_data.TargetTypeKey == null)
                    _data.TargetTypeKey = deviceType;
                avalableDeviceTypes.Add(deviceType,
                    new DeviceTypeDescriptor(setDevice.Model, setDevice.DeviceCommonType, setDevice.DeviceManufacturer));
            }
            _avalableDeviceTypes = avalableDeviceTypes;

            // Выбираем первый тип устройства
            _data.CheckTypeKey = DeviceTypes.First();
            _data.TargetType = avalableDeviceTypes[_data.CheckTypeKey];

            if (_data.TargetTypeKey != null)
            {
                _result.TargetDevice = new DeviceDescriptor(_data.TargetType);
                _checks = _method.MethodsForType(_data.TargetTypeKey);
                var channels = new List<ChannelDescriptor>();
                var channelsKeys = new Dictionary<ChannelDescriptor, string>();
                // пулл выбранного типа устройства
                var channelsPool = _propertyPool.ByKey(_data.TargetTypeKey);
                foreach (var chKey in channelsPool.GetAllKeys())
                {// перебор каналов выбранного типа устройства
                    var oneChannel = channelsPool.ByKey(chKey).GetProperty<ChannelDescriptor>(BasicKeys.KeyChannel);
                    if(oneChannel == null)
                        continue;
                    channels.Add(oneChannel);
                    channelsKeys.Add(oneChannel, chKey);
                }
                Channels = channels;
                _channelKeys = channelsKeys;
                // выбираем первый тип канала
                _result.Channel = Channels.First();
                SelectedMethodKey = Methods.First();
            }
        }

        /// <summary>
        /// Загрузить набор допустимых эталонов
        /// </summary>
        /// <param name="settings"></param>
        /// <remarks>
        /// 1) Выбрать устройства, имеющие измерительные каналы такого же типа что и проверяемый
        /// 2) Выбрать подходящие по диапазону и точности каналы
        /// </remarks>
        private void LoadAvalableEthalons(IMainSettings settings)
        {
            var avalableEthalonTypes = new Dictionary<string, DeviceTypeDescriptor>();
            _data.EthalonTypeKey = null;
            var channelKey = _channelKeys[SelectedChannel];
            var ethalons = _propertyPool.ByKey(_data.TargetTypeKey).ByKey(channelKey).GetProperty<List<string>>(CommonPropertyKeys.KeyEthalons);
            foreach (var deviceEthalon in ethalons)
            {
                var setDevice = settings.Devices.FirstOrDefault(el => el.Key == deviceEthalon);
                if (_data.EthalonTypeKey == null)
                {
                    _data.EthalonTypeKey = deviceEthalon;
                }
                if (setDevice == null)
                {
                    if (_data.EthalonTypeKey == UserEthalonChannel.Key)
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
            if (_data.TargetTypeKey != null)
            {
                if (_data.EthalonTypeKey != UserEthalonChannel.Key)
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
        /// Измерительные каналы устройства
        /// </summary>
        public IEnumerable<ChannelDescriptor> Channels
        {
            get { return _channels; }
            set { _channels = value; }
        }

        /// <summary>
        /// Дострупные для выбранного типа устройства методики
        /// </summary>
        public IEnumerable<string> Methods
        {
            get { return _checks.Keys; }
        }

        /// <summary>
        /// Доступные типы эталонов
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
        /// Атмосферное давление, гПа
        /// </summary>
        public string AtmospherePressure { get { return _result.AtmospherePressure; } set { _result.AtmospherePressure = value; } }

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
            get { return _data.TargetType; }
            private set
            {
                if (_data.TargetType == value)
                    return;
                _data.TargetType = value;
                _result.TargetDevice.DeviceType = _data.TargetType;
                _checks = _method.MethodsForType(_data.TargetTypeKey);
                if (!_checks.ContainsKey(_data.CheckTypeKey) && _checks.Count > 0)
                {
                    SelectedMethodKey = _checks.First().Key;
                }
                var channelKey = _channelKeys[SelectedChannel];
                var properties = _propertyPool.ByKey(_data.TargetTypeKey).ByKey(channelKey);
                CustomSettings = _checks[_data.CheckTypeKey].GetCustomConfig(properties);
            }
        }

        /// <summary>
        /// Тип устройства строка 
        /// </summary>
        public string SelectedDeviceTypeKey
        {
            get { return _data.TargetTypeKey; }
            set
            {
                if (_data.TargetTypeKey == value)
                    return;
                _data.TargetTypeKey = value;
                SelectedDeviceType = _avalableDeviceTypes[_data.TargetTypeKey];
            }
        }

        /// <summary>
        /// Производитель
        /// </summary>
        public string Manufacturer
        {
            get
            {
                return _result.TargetDevice.DeviceType.DeviceManufacturer;
            }
        }

        /// <summary>
        /// Заказчик
        /// </summary>
        public string Client
        {
            get { return _result.Client; }
            set { _result.Client = value; }
        }

        /// <summary>
        /// Заводской номер
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
        public string SelectedMethodKey
        {
            get { return _data.CheckTypeKey; }
            set
            {
                _data.CheckTypeKey = value;
                _result.CheckType = value;
                SelectedMethod = _checks[_data.CheckTypeKey];
            }
        }

        /// <summary>
        /// Выбранная методика
        /// </summary>
        public ICheckMethod SelectedMethod
        {
            get { return _selectedCheckType; }
            private set
            {
                _selectedCheckType = value;
                _result.CheckType = _data.CheckTypeKey;
                var channelKey = _channelKeys[SelectedChannel];
                var properties = _propertyPool.ByKey(_data.TargetTypeKey).ByKey(channelKey);
                CustomSettings = SelectedMethod.GetCustomConfig(properties);
                SelectedMethod.Init(CustomSettings); //todo maybe move out
                OnSelectedMethodChanged();
            }
        }

        /// <summary>
        /// Выбранный измерительный канал
        /// </summary>
        public ChannelDescriptor SelectedChannel
        {
            get { return _result.Channel; }
            set
            {
                if (value == _result.Channel)
                    return;
                _result.Channel = value;
                var channelKey = _channelKeys[value];
                var properties = _propertyPool.ByKey(_data.TargetTypeKey).ByKey(channelKey);
                CustomSettings = SelectedMethod.GetCustomConfig(properties);
                SelectedMethod.Init(CustomSettings); //todo maybe move out
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
            get { return _data.EthalonTypeKey; }
            set
            {
                _data.EthalonTypeKey = value;
                IsAnalogEthalon = _data.EthalonTypeKey == UserEthalonChannel.Key;
                SelectedEthalonType = _avalableEthalonTypes[_data.EthalonTypeKey];
            }
        }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public DeviceTypeDescriptor SelectedEthalonType
        {
            get { return _data.EthalonType; }
            set
            {
                _data.EthalonType = value;
                if (!IsAnalogEthalon)
                    Ethalon.DeviceType = _data.EthalonType;
                OnSelectedEthalonTypeChanged();
            }
        }

        /// <summary>
        /// Эталоном выбран прибор без элетронного интерфейса
        /// </summary>
        public bool IsAnalogEthalon
        {
            get { return _data.IsAnalogEthalon; }
            set
            {
                _data.IsAnalogEthalon = value;
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
                Ethalon.DeviceType = new DeviceTypeDescriptor(_data.Ethalon.DeviceType.Model, _data.Ethalon.DeviceType.DeviceCommonType, value);
            }
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
            get { return _data.Ethalon; }
            protected set { _data.Ethalon = value; }
        }

        #endregion

        #region Custom settings

        /// <summary>
        /// Настройка конкретной методики
        /// </summary>
        public object CustomSettings { get; set; }

        #endregion

        #region Events

        public event EventHandler SelectedMethodChanged;

        protected virtual void OnSelectedMethodChanged()
        {
            EventHandler handler = SelectedMethodChanged;
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
