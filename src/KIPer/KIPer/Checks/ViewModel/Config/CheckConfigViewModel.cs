using System;
using System.Collections.Generic;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Checks;
using GalaSoft.MvvmLight;
using KipTM.Checks;
using KipTM.Checks.ViewModel.Config;
using KipTM.Interfaces.Channels;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel.Checks.Config
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CheckConfigViewModel : ViewModelBase
    {
        private readonly CheckConfigDevice _model;
        private readonly CustomConfigFactory _customConfigFactory;
        private SelectChannelViewModel _checkDeviceChanel;
        private SelectChannelViewModel _ethalonChanel;
        private ICustomSettingsViewModel _customSetiings;
        private bool _isCustomSettingsAvailable;


        /// <summary>
        /// Initializes a new instance of the CheckConfigViewModel class.
        /// </summary>
        public CheckConfigViewModel(CheckConfigDevice model, IChannelsFactory channelFactory, CustomConfigFactory customConfigFactory)
        {
            _model = model;
            _customConfigFactory = customConfigFactory;
            CustomSetiings = _customConfigFactory.GetCustomSettings(_model.CustomSettings);
            _model.SelectedChannelChanged += _model_SelectedChannelChanged;
            _model.SelectedMethodChanged += ModelSelectedMethodChanged;
            _model.SelectedEthalonTypeChanged += _model_SelectedEthalonTypeChanged;
            _checkDeviceChanel = new SelectChannelViewModel(channelFactory.GetChannels());
            _model.TargetTransportChannel = _checkDeviceChanel.SelectedChannel;
            _checkDeviceChanel.ChannelTypeChanget += _checkDeviceChanel_ChannelTypeChanget;
            _ethalonChanel = new SelectChannelViewModel(channelFactory.GetChannels());
            _model.EthalonTransportChannel = _ethalonChanel.SelectedChannel;
            _ethalonChanel.ChannelTypeChanget += _ethalonChanel_ChannelTypeChanget;
        }

        #region Обработка событий изменения конфигурации

        /// <summary>
        /// Изменение конфигурации канала связи с эталоном
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ethalonChanel_ChannelTypeChanget(object sender, EventArgs e)
        {
            _model.EthalonTransportChannel = _ethalonChanel.SelectedChannel;
            OnEthalonDeviseChannelChanged();
        }

        /// <summary>
        /// Изменение конфигурации канала связи с объектом контроля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _checkDeviceChanel_ChannelTypeChanget(object sender, EventArgs e)
        {
            _model.TargetTransportChannel = _checkDeviceChanel.SelectedChannel;
            OnCheckedDeviseChannelChanged();
        }

        /// <summary>
        /// Изменение типа эталонного канала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _model_SelectedEthalonTypeChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("SelectedEthalonType");
            RaisePropertyChanged("IsAnalogEthalon");
            RaisePropertyChanged("IsNoAnalogEthalon");
        }

        /// <summary>
        /// Измененеие типа методики
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelSelectedMethodChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("SelectedCheckType");
            RaisePropertyChanged("SelectedChannel");
            RaisePropertyChanged("CheckDeviceChanel");
        }

        /// <summary>
        /// Изменение измерительного канала связи с эталоном
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _model_SelectedChannelChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("SelectedChannel");
        }

        #endregion

        #region Перечисления

        /// <summary>
        /// Каналы устройства
        /// </summary>
        public IEnumerable<ChannelDescriptor> Channels { get { return _model.Channels; } }

        /// <summary>
        /// Дострупные для выбранного типа устройства методики
        /// </summary>
        public IEnumerable<string> CheckTypes
        {
            get { return _model.Methods; }
        }

        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IEnumerable<string> EthalonTypes { get { return _model.EthalonTypes; } }

        #endregion

        #region Условия проверки
        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime CheckDateTime {
            get { return _model.CheckDateTime; }
            set
            {
                _model.CheckDateTime = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Атмосферное давление, гПа
        /// </summary>
        public string AtmospherePressure
        {
            get { return _model.AtmospherePressure; }
            set
            {
                _model.AtmospherePressure = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Температура
        /// </summary>
        public string Temperature
        {
            get { return _model.Temperature; }
            set
            {
                _model.Temperature = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Влажность
        /// </summary>
        public string Humidity
        {
            get { return _model.Humidity; }
            set
            {
                _model.Humidity = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Настройки проверяемого устройства

        /// <summary>
        /// Тип устройства
        /// </summary>
        public string SelectedDeviceType
        {
            get { return _model.SelectedDeviceType.Model; }
        }

        /// <summary>
        /// Заказчик
        /// </summary>
        public string Client
        {
            get { return _model.Client; }
            set
            {
                _model.Client = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Производитель
        /// </summary>
        public string Manufacturer
        {
            get { return _model.Manufacturer; }
        }

        /// <summary>
        /// Заводской номер
        /// </summary>
        public string SerialNumber
        {
            get { return _model.SerialNumber; }
            set
            {
                _model.SerialNumber = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime PreviousCheckTime
        {
            get { return _model.PreviousCheckTime; }
            set
            {
                _model.PreviousCheckTime = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Выбранная методика
        /// </summary>
        public string SelectedCheckType
        {
            get { return _model.SelectedMethodKey; }
            set
            {
                _model.SelectedMethodKey = value;
                CustomSetiings = _customConfigFactory.GetCustomSettings(_model.CustomSettings);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Выбранный Измерительный канал
        /// </summary>
        public ChannelDescriptor SelectedChannel
        {
            get { return _model.SelectedChannel; }
            set
            {
                _model.SelectedChannel = value;
                CustomSetiings = _customConfigFactory.GetCustomSettings(_model.CustomSettings);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Настройки канала тестируемого прибора
        /// </summary>
        public SelectChannelViewModel CheckDeviceChanel
        {
            get { return _checkDeviceChanel; }
            set
            {
                _checkDeviceChanel = value;
            }
        }

        #endregion

        #region Настройки эталона

        /// <summary>
        /// Тип устройства
        /// </summary>
        public string SelectedEthalonType
        {
            get { return _model.SelectedEthalonTypeKey; }
            set
            {
               _model.SelectedEthalonTypeKey = value;
               RaisePropertyChanged();
            }
        }

        public bool IsAnalogEthalon
        {
            get { return _model.IsAnalogEthalon; }
            set
            {
                _model.IsAnalogEthalon =  value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsNoAnalogEthalon");
            }
        }

        public bool IsNoAnalogEthalon
        {
            get { return !IsAnalogEthalon; }
        }

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string EthalonDeviceType
        {
            get { return _model.EthalonDeviceType; }
            set
            {
                _model.EthalonDeviceType = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Производитель эталона
        /// </summary>
        public string EthalonManufacturer
        {
            get { return _model.EthalonManufacturer; }
            set
            {
                _model.EthalonManufacturer = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string EthalonSerialNumber
        {
            get { return _model.EthalonSerialNumber; }
            set
            {
                _model.EthalonSerialNumber = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime EthalonPreviousCheckTime
        {
            get { return _model.EthalonPreviousCheckTime; }
            set
            {
                _model.EthalonPreviousCheckTime = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Настройки канала эталона
        /// </summary>
        public SelectChannelViewModel EthalonChanel
        {
            get { return _ethalonChanel; }
            set { _ethalonChanel = value; }
        }

        #endregion

        #region Настройки каналов

        public ITransportChannelType GetCheckedDeviseChannel()
        {
            return CheckDeviceChanel.GetSelectedChannelType();
        }

        public ITransportChannelType GetEthalonDeviseChannel()
        {
            return EthalonChanel.GetSelectedChannelType();
        }

        public event EventHandler CheckedDeviseChannelChanged;

        protected virtual void OnCheckedDeviseChannelChanged()
        {
            EventHandler handler = CheckedDeviseChannelChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler EthalonDeviseChannelChanged;

        protected virtual void OnEthalonDeviseChannelChanged()
        {
            EventHandler handler = EthalonDeviseChannelChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        #endregion

        #region Настройка конкретного типа проверки конкретного устройства

        /// <summary>
        /// Настройка конкретной методики
        /// </summary>
        public ICustomSettingsViewModel CustomSetiings
        {
            get { return _customSetiings; }
            set
            {
                Set(ref _customSetiings, value);
                IsCustomSettingsAvailable = _customSetiings != null;
            }
        }

        /// <summary>
        /// Наличие настройки конкретной методики
        /// </summary>
        public bool IsCustomSettingsAvailable
        {
            get { return _isCustomSettingsAvailable; }
            set { Set(ref _isCustomSettingsAvailable, value); }
        }

        #endregion

        #region Разрушение, отчистка ресурсов

        public override void Cleanup()
        {
            _checkDeviceChanel.ChannelTypeChanget -= _checkDeviceChanel_ChannelTypeChanget;
            _ethalonChanel.ChannelTypeChanget -= _ethalonChanel_ChannelTypeChanget;
            _model.SelectedChannelChanged -= _model_SelectedChannelChanged;
            _model.SelectedMethodChanged -= ModelSelectedMethodChanged;
            _model.SelectedEthalonTypeChanged -= _model_SelectedEthalonTypeChanged;
            base.Cleanup();
        }

        #endregion
    }
}