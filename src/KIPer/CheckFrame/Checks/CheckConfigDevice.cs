using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ArchiveData.DTO;
using CheckFrame.Archive;
using CheckFrame.Channels;
using CheckFrame.Checks;
using CheckFrame.Model.Channels;
using CheckFrame.Model.Checks;
using KipTM.Archive;
using KipTM.Interfaces;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.Checks;
using KipTM.Model.TransportChannels;
using KipTM.Settings;

namespace KipTM.Checks
{
    /// <summary>
    /// Конфигурация проверки конкретного типа
    /// </summary>
    //TODO: пока не используется но необходимо перейти на работу с конкретным типом проверки
    public class CheckConfigDevice
    {
        #region Внутренние переменные
        /// <summary>
        /// Набор доступных типов проверок
        /// </summary>
        private IDictionary<string, ICheckMethod> _methods;
        /// <summary>
        /// Набр дополнительных настроек
        /// </summary>
        private readonly IPropertyPool _propertyPool;
        /// <summary>
        /// Выбранная методика проверки
        /// </summary>
        private ICheckMethod _selectedCheckType;

        /// <summary>
        /// Все доступные типы устройств и их описатели
        /// </summary>
        private IEnumerable<DeviceTypeDescriptor> _allDeviceTypes;
        /// <summary>
        /// Доступные типы эталоннных устройств
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
        private readonly IDictionary<ChannelDescriptor, IPropertyPool> _channelKeys;

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

        #region Конструктор

        /// <summary>
        /// Конфигурация проверки
        /// </summary>
        /// <param name="data"></param>
        /// <param name="methods"></param>
        /// <param name="allDeviceTypes"></param>
        /// <param name="propertyPool"></param>
        /// <param name="result"></param>
        public CheckConfigDevice(CheckConfigData data, IDictionary<string, ICheckMethod> methods, IEnumerable<DeviceTypeDescriptor> allDeviceTypes,
            IPropertyPool propertyPool, TestResult result)
        {
            _data = data;
            _methods = methods;
            _selectedCheckType = _methods.Values.FirstOrDefault();
            _allDeviceTypes = allDeviceTypes;
            _result = result;
            _propertyPool = propertyPool;
            _channelKeys = GetChannels(_propertyPool, _data.TargetType);
            _channels = _channelKeys.Keys;
            _result.TargetDevice.Channel = _channels.FirstOrDefault();

            _avalableEthalonTypes = GetAvailableEthalons(_data.TargetType, propertyPool, _result.TargetDevice.Channel, _allDeviceTypes);
            var etnChannels = GetChannels(_propertyPool, _data.Ethalon.DeviceType);
            _data.Ethalon = new DeviceDescriptor(_avalableEthalonTypes.Values.FirstOrDefault());
            _result.TargetDevice.Device = new DeviceDescriptor(data.TargetType);
            _result.Ethalons.Add(_result.TargetDevice.Channel, new DeviceWithChannel()
            {
                Device = _data.Ethalon,
                Channel = etnChannels.Keys.FirstOrDefault()//TODO реализовать выбор канала
            });
            UpdateCustomMethodSettings(_result.TargetDevice.Channel);
        }
        #endregion

        #region Перечисления
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
            get { return _methods.Keys; }
        }

        /// <summary>
        /// Доступные типы эталоных каналов (со всех доступных устройств)
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
        }

        /// <summary>
        /// Производитель
        /// </summary>
        public string Manufacturer
        {
            get
            {
                return _result.TargetDevice.Device.DeviceType.DeviceManufacturer;
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
            get { return _result.TargetDevice.Device.SerialNumber; }
            set { _result.TargetDevice.Device.SerialNumber = value; }
        }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime PreviousCheckTime
        {
            get { return _result.TargetDevice.Device.PreviousCheckTime; }
            set { _result.TargetDevice.Device.PreviousCheckTime = value; }
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
                SelectedMethod = _methods[_data.CheckTypeKey];
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
                UpdateCustomMethodSettings(SelectedChannel);
                OnSelectedMethodChanged();
            }
        }

        /// <summary>
        /// Выбранный измерительный канал объекта контроля
        /// </summary>
        public ChannelDescriptor SelectedChannel
        {
            get { return _result.TargetDevice.Channel; }
            set
            {
                if (value == _result.TargetDevice.Channel)
                    return;
                _result.TargetDevice.Channel = value;
                UpdateCustomMethodSettings(SelectedChannel);
                UpdateEthalonChannel(_data.TargetType, SelectedChannel);
                OnSelectedChannelChanged();
            }
        }

        /// <summary>
        /// Канал подключения к проверяемому устройству
        /// </summary>
        public ITransportChannelType TargetTransportChannel { get; set; }

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
        /// Тип устройства содержащего выбранный эталонный канал
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
        /// Тип устройства - эталона
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
        /// Эталон
        /// </summary>
        public DeviceDescriptor Ethalon
        {
            get { return _data.Ethalon; }
            protected set { _data.Ethalon = value; }
        }

        /// <summary>
        /// Канал подключения к эталону
        /// </summary>
        public ITransportChannelType EthalonTransportChannel { get; set; }

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


        public event EventHandler AvailableEthalonTypeChanged;

        protected virtual void OnAvailableEthalonTypeChanged()
        {
            EventHandler handler = AvailableEthalonTypeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler SelectedEthalonTypeChanged;

        protected virtual void OnSelectedEthalonTypeChanged()
        {
            EventHandler handler = SelectedEthalonTypeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion

        #region Services

        /// <summary>
        /// Обновить эсклюзивные настройки методики
        /// </summary>
        private void UpdateCustomMethodSettings(ChannelDescriptor targetChannel)
        {
            var properties = _channelKeys[targetChannel];
            CustomSettings = SelectedMethod.GetCustomConfig(properties);
            SelectedMethod.Init(CustomSettings); //todo maybe move out
        }

        /// <summary>
        /// Пререпроверить подходит ли этому каналу выбранный тип эталонного канала,
        /// если нет - выбрать подходящий
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="selectedChannel"></param>
        private void UpdateEthalonChannel(DeviceTypeDescriptor targetType, ChannelDescriptor selectedChannel)
        {
            _avalableEthalonTypes = GetAvailableEthalons(targetType, _propertyPool, selectedChannel, _allDeviceTypes);
            OnAvailableEthalonTypeChanged();
            if (_avalableEthalonTypes.Values.Contains(_data.Ethalon.DeviceType))
                return;
            SelectedEthalonTypeKey = _avalableEthalonTypes.Keys.FirstOrDefault();
        }

        /// <summary>
        /// Получить коллекцию опистелей измерительных каналов устройства
        /// </summary>
        /// <param name="propertyPool"></param>
        /// <param name="targetTypeKey">Ключ типа устройства</param>
        /// <returns>Справочник описателей каналов и их ключей</returns>
        private static Dictionary<ChannelDescriptor, IPropertyPool> GetChannels(IPropertyPool propertyPool, DeviceTypeDescriptor targetType)
        {
            var channelKeys = new Dictionary<ChannelDescriptor, IPropertyPool>();
            // свойства выбранного типа устройства
            var channelsPool = propertyPool.ByKey(targetType.TypeKey);
            foreach (var chKey in channelsPool.GetAllKeys())
            {
                // перебор каналов выбранного типа устройства
                var oneChannel = channelsPool.ByKey(chKey).GetProperty<ChannelDescriptor>(BasicKeys.KeyChannel);
                if (oneChannel == null)
                    continue;

                channelKeys.Add(oneChannel, channelsPool.ByKey(chKey));
            }
            return channelKeys;
        }

        /// <summary>
        /// Получить коллекцию опистелей измерительных каналов устройства
        /// </summary>
        /// <param name="propertyPool"></param>
        /// <param name="targetType">тип устройства</param>
        /// <returns>Справочник описателей каналов и их ключей</returns>
        private static Dictionary<ChannelDescriptor, string> GetChannelsKeys(IPropertyPool propertyPool, DeviceTypeDescriptor targetType)
        {
            var channelKeys = new Dictionary<ChannelDescriptor, string>();
            // свойства выбранного типа устройства
            var channelsPool = propertyPool.ByKey(targetType.TypeKey);
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
        /// Получить набор подходящих каналов эталонов
        /// </summary>
        /// <param name="targetDev">тип проверяемого устройства</param>
        /// <param name="propertyPool"></param>
        /// <param name="selectedChannel">выбраный канал</param>
        /// <param name="allTypes"></param>
        /// <returns>Коллекцию формата (ключ изметительного канала - описатель устройства-носителя канала)</returns>
        private static IDictionary<string, DeviceTypeDescriptor> GetAvailableEthalons(DeviceTypeDescriptor targetDev, IPropertyPool propertyPool,
            ChannelDescriptor selectedChannel, IEnumerable<DeviceTypeDescriptor> allTypes) //IMainSettings settings, 
        {
            var avalableEthalonTypes = new Dictionary<string, DeviceTypeDescriptor>();
            foreach (var dev in allTypes)
            {// выбираем очередное устройство
                //исключить проверяемое устройство
                if (dev == targetDev)
                    continue;

                var channels = GetChannelsKeys(propertyPool, dev);
                foreach (var channel in channels)
                {// перебираем каналы выбранного устройства
                    if (!CheckEthalonChannel(selectedChannel, channel.Key))
                        continue;

                    avalableEthalonTypes.Add(channel.Value, dev);
                }
            }
            avalableEthalonTypes.Add(UserEthalonChannel.Key, UserEthalonChannel.Descriptor);
            return avalableEthalonTypes;
        }

        /// <summary>
        /// Проверить подходит ли эталонный канал выбранному
        /// </summary>
        /// <param name="selectedChannel">Выбранный канал</param>
        /// <param name="ethalonChannel">Эталонный канал</param>
        /// <returns>True - подходит</returns>
        private static bool CheckEthalonChannel(ChannelDescriptor selectedChannel, ChannelDescriptor ethalonChannel)
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
        #endregion
    }
}
