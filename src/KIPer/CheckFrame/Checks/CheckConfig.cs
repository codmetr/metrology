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
        /// Доступные типы устройств эталонов
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
            // заполнение списка поддерживаемых устройств
            _data.TargetTypeKey = null;
            var avalableDeviceTypes = GetAllAvailableDeviceTypes(settings, dictionaries);

            if (avalableDeviceTypes == null)
                throw new NullReferenceException(string.Format("Не удалось получить список доступных типов устройств"));

            _avalableDeviceTypes = avalableDeviceTypes;

            // Выбираем первый тип устройства объекта контроля
            _data.TargetTypeKey = avalableDeviceTypes.Keys.FirstOrDefault();
            _data.TargetType = _avalableDeviceTypes[_data.TargetTypeKey];

            if (_data.TargetTypeKey == null)
                return;
            //Заполняем по выбранному устройству набор методик, каналов и т.п.
            ConfigMethodForDevice(_data.TargetType, _data.TargetTypeKey);
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
            var selectedChannel = SelectedChannel;
            var targetDevKey = _data.TargetTypeKey;

            var avalableEthalonTypes = GetAvailableEthalons(settings, targetDevKey, selectedChannel);

            _avalableEthalonTypes = avalableEthalonTypes;
            _data.EthalonTypeKey = _avalableEthalonTypes.FirstOrDefault().Key;
            if (_data.EthalonTypeKey == UserEthalonChannel.Key)
            {
                _data.EthalonType = new DeviceTypeDescriptor("", "", "");
            }
            else
            {
                _data.EthalonType = _avalableEthalonTypes.FirstOrDefault().Value;
            }
            _data.Ethalon = new DeviceDescriptor(_data.EthalonType);


            /*
            _data.EthalonTypeKey = null;
            var channelKey = _channelKeys[SelectedChannel];
            var ethalons = _propertyPool.ByKey(_data.TargetTypeKey).ByKey(channelKey).GetProperty<List<string>>(CommonPropertyKeys.KeyEthalons);
            foreach (var deviceEthalon in ethalons)
            {
                var setDevice = GetSettingsDevice(settings, deviceEthalon);
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
            }*/
        }

        /// <summary>
        /// Получить набор подходящих каналов эталонов
        /// </summary>
        /// <param name="settings">настройки</param>
        /// <param name="targetDevKey">ключ типа проверяемого устройства</param>
        /// <param name="selectedChannel">выбраный канал</param>
        /// <returns></returns>
        private Dictionary<string, DeviceTypeDescriptor> GetAvailableEthalons(IMainSettings settings, string targetDevKey, ChannelDescriptor selectedChannel)
        {
            var avalableEthalonTypes = new Dictionary<string, DeviceTypeDescriptor>();
            foreach (var dev in _avalableDeviceTypes)
            {
                //исключить проверяемое устройство
                if (dev.Key == targetDevKey)
                    continue;

                var channels = GetChannels(dev.Key);
                foreach (var channel in channels)
                {
                    if (!CheckEthalonChannel(selectedChannel, channel.Key))
                        continue;

                    //получение настроек
                    var setDevice = GetSettingsDevice(settings, channel.Value);
                    if (setDevice == null)
                    {
                        avalableEthalonTypes.Add(channel.Value, null);
                        continue;
                    }

                    avalableEthalonTypes.Add(channel.Value,
                        new DeviceTypeDescriptor(setDevice.Model, setDevice.DeviceCommonType, setDevice.DeviceManufacturer));
                }
            }
            avalableEthalonTypes.Add(UserEthalonChannel.Key,
                new DeviceTypeDescriptor("Аналоговый прибор", "Приборы без аппаратного интерфейса", ""));
            return avalableEthalonTypes;
        }

        /// <summary>
        /// Проверить подходит ли эталонный канал выбранному
        /// </summary>
        /// <param name="selectedChannel">Выбранный канал</param>
        /// <param name="ethalonChannel">Эталонный канал</param>
        /// <returns>True - подходит</returns>
        private bool CheckEthalonChannel(ChannelDescriptor selectedChannel, ChannelDescriptor ethalonChannel)
        {
            //проверка типа измерительного канала
            if (selectedChannel.TypeChannel != ethalonChannel.TypeChannel)
                return false;
            //проверка нарпавленности измерительного канала
            if (selectedChannel.Order == ethalonChannel.Order)
                return false;
            //проверка диапазона измерительного канала
            if (selectedChannel.Min < ethalonChannel.Min || selectedChannel.Max > ethalonChannel.Max)
                return false;

            //проверка допуска измерительного канала
            if (selectedChannel.Error < ethalonChannel.Error)
                return false;
            return true;
        }

        /// <summary>
        /// Сконфигурировать методику и каналы по заданному типу устройств
        /// </summary>
        /// <param name="targetType">Описатель типа объекта контроля</param>
        /// <param name="targetTypeKey">Ключ описателя типа объекта контроля</param>
        private void ConfigMethodForDevice(DeviceTypeDescriptor targetType, string targetTypeKey)
        {
            _result.TargetDevice = new DeviceDescriptor(targetType);
            _checks = _method.MethodsForType(targetTypeKey);
            // заполняем набор измерительных каналов
            var channelKeys = GetChannels(targetTypeKey);
            Channels = channelKeys.Keys;
            _channelKeys = channelKeys;
            // выбираем первый тип канала
            _result.Channel = Channels.First();
            // выбираем первую методику
            SelectedMethodKey = Methods.First();
            var channelKey = _channelKeys[SelectedChannel];
            var properties = _propertyPool.ByKey(_data.TargetTypeKey).ByKey(channelKey);
            CustomSettings = SelectedMethod.GetCustomConfig(properties);
        }

        /// <summary>
        /// Получить коллекцию опистелей измерительных каналов устройства
        /// </summary>
        /// <param name="targetTypeKey">Ключ типа устройства</param>
        /// <returns>Справочник описателей каналов и их ключей</returns>
        private Dictionary<ChannelDescriptor, string> GetChannels(string targetTypeKey)
        {
            var channelKeys = new Dictionary<ChannelDescriptor, string>();
            // свойства выбранного типа устройства
            var channelsPool = _propertyPool.ByKey(targetTypeKey);
            foreach (var chKey in channelsPool.GetAllKeys())
            {
                // перебор каналов выбранного типа устройства
                var oneChannel = channelsPool.ByKey(chKey).GetProperty<ChannelDescriptor>(BasicKeys.KeyChannel);
                if (oneChannel == null)
                    continue;

                channelKeys.Add(oneChannel, chKey);
            }
            return channelKeys;
        }

        /// <summary>
        /// Получить список доступных типов устройств (объектов контроля)
        /// </summary>
        /// <param name="settings">Настройки</param>
        /// <param name="dictionaries">Справочник функционала</param>
        /// <returns></returns>
        private Dictionary<string, DeviceTypeDescriptor> GetAllAvailableDeviceTypes(IMainSettings settings, DictionariesPool dictionaries)
        {
            var avalableDeviceTypes = new Dictionary<string, DeviceTypeDescriptor>();
            foreach (var deviceType in dictionaries.DeviceTypes)
            {
                var setDevice = GetSettingsDevice(settings, deviceType);
                if (setDevice == null)
                    continue;
                avalableDeviceTypes.Add(deviceType,
                    new DeviceTypeDescriptor(setDevice.Model, setDevice.DeviceCommonType, setDevice.DeviceManufacturer));
            }
            return avalableDeviceTypes;
        }

        /// <summary>
        /// Получить настройки устройства
        /// </summary>
        /// <param name="settings">контейнер настроек</param>
        /// <param name="deviceType">Ключ типа устройства</param>
        /// <returns></returns>
        private static DeviceTypeSettings GetSettingsDevice(IMainSettings settings, string deviceType)
        {
            return settings.Devices.First(el => el.Key == deviceType);
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

                if (_data.TargetTypeKey == null)
                    return;
                //Заполняем по выбранному устройству набор методик, каналов и т.п.
                ConfigMethodForDevice(_data.TargetType, _data.TargetTypeKey);
                /*
                _result.TargetDevice.DeviceType = _data.TargetType;
                _checks = _method.MethodsForType(_data.TargetTypeKey);
                if (!_checks.ContainsKey(_data.CheckTypeKey) && _checks.Count > 0)
                {
                    SelectedMethodKey = _checks.First().Key;
                }
                var channelKey = _channelKeys[SelectedChannel];
                var properties = _propertyPool.ByKey(_data.TargetTypeKey).ByKey(channelKey);
                CustomSettings = _checks[_data.CheckTypeKey].GetCustomConfig(properties);*/
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
        /// Выбранный измерительный канал объекта контроля
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
