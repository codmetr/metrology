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
        private IDictionary<string, ICheckMethod> _checks;
        /// <summary>
        /// Набр дополнительных настроек
        /// </summary>
        private readonly IPropertyPool _propertyPool;
        /// <summary>
        /// Выбранная методика проверки
        /// </summary>
        private ICheckMethod _selectedCheckType;

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

        #region Конструктор

        /// <summary>
        /// Конфигурация проверки
        /// </summary>
        /// <param name="device"></param>
        /// <param name="devTypeKey"></param>
        /// <param name="methods"></param>
        /// <param name="ethalons"></param>
        /// <param name="propertyPool"></param>
        /// <param name="result"></param>
        public CheckConfigDevice(DeviceTypeDescriptor device, string devTypeKey,
            IDictionary<string, ICheckMethod> methods, IDictionary<string, DeviceTypeDescriptor> ethalons, object customSettings,
            IPropertyPool propertyPool, TestResult result)
        {
            _data = new CheckConfigData() {TargetType = device, TargetTypeKey = devTypeKey};
            _checks = methods;
            _selectedCheckType = _checks.Values.FirstOrDefault();
            _avalableEthalonTypes = ethalons;
            Ethalon = new DeviceDescriptor(_avalableEthalonTypes.Values.FirstOrDefault());
            _result = result;
            _propertyPool = propertyPool;
            SelectedChannel = Channels.FirstOrDefault();
            UpdateCustomSettings(devTypeKey, SelectedChannel);
        }
        #endregion

        #region Инициализация

        /// <summary>
        /// Обновить эсклюзивные настройки методики
        /// </summary>
        private void UpdateCustomSettings(string targetDevKey, ChannelDescriptor targetChannel)
        {
            var channelKey = _channelKeys[targetChannel];
            var properties = _propertyPool.ByKey(targetDevKey).ByKey(channelKey);
            CustomSettings = SelectedMethod.GetCustomConfig(properties);
            SelectedMethod.Init(CustomSettings); //todo maybe move out
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
                UpdateCustomSettings(_data.TargetTypeKey, SelectedChannel);
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
                UpdateCustomSettings(_data.TargetTypeKey, SelectedChannel);
                UpdateEthalonChannel(_data.TargetTypeKey, SelectedChannel);
                OnSelectedChannelChanged();
            }
        }

        /// <summary>
        /// Пререпроверить подходит ли этому каналу выбранный тип эталонного канала,
        /// если нет - выбрать подходящий
        /// </summary>
        /// <param name="targetTypeKey"></param>
        /// <param name="selectedChannel"></param>
        private void UpdateEthalonChannel(string targetTypeKey, ChannelDescriptor selectedChannel)
        {
            //TODO: Пререпроверить подходит ли этому каналу выбранный тип эталонного канала
            throw new NotImplementedException();
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

        public event EventHandler SelectedEthalonTypeChanged;

        protected virtual void OnSelectedEthalonTypeChanged()
        {
            EventHandler handler = SelectedEthalonTypeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
