using System;
using System.Collections.Generic;
using System.Linq;
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
        private ICustomSettingsViewModel _customSetiings;
        private bool _isCustomSettingsAvailable;

        public Dictionary<ChannelDescriptor, EthalonConfigViewModel> _ethalons;


        /// <summary>
        /// Визуальная модель конфигурации конкретного типа проверки
        /// </summary>
        public CheckConfigViewModel(CheckConfigDevice model, IChannelsFactory channelFactory, CustomConfigFactory customConfigFactory)
        {
            _model = model;
            _customConfigFactory = customConfigFactory;
            CustomSetiings = _customConfigFactory.GetCustomSettings(_model.CustomSettings);

            _model.SelectedChannelChanged += _model_SelectedChannelChanged;
            _model.SelectedMethodChanged += ModelSelectedMethodChanged;

            _checkDeviceChanel = new SelectChannelViewModel(channelFactory.GetChannels());
            _checkDeviceChanel.ChannelTypeChanget += _checkDeviceChanel_ChannelTypeChanget;

            var echalon = _model.EthalonWithCh;
            _ethalons = _model.Channels.ToDictionary(elKey => elKey,
                elV => new EthalonConfigViewModel(echalon, _model.GetAvailableEthalons(elV),
                    channelFactory.GetChannels()));

            _model.TargetTransportChannel = _checkDeviceChanel.SelectedChannel;
            _model.EthalonTransportChannel = _ethalons[_model.SelectedChannel].EthalonChanel.SelectedChannel;
        }

        #region Обработка событий изменения конфигурации

        /// <summary>
        /// Изменение конфигурации канала связи с объектом контроля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _checkDeviceChanel_ChannelTypeChanget(object sender, EventArgs e)
        {
            _model.TargetTransportChannel = _checkDeviceChanel.SelectedChannel;
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
                _model.EthalonTransportChannel = EthalonOneCh.EthalonChanel.SelectedChannel;
                CustomSetiings = _customConfigFactory.GetCustomSettings(_model.CustomSettings);
                RaisePropertyChanged();
                RaisePropertyChanged("EthalonOneCh");
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

        public EthalonConfigViewModel EthalonOneCh { get { return _ethalons[SelectedChannel]; } }

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
            _model.SelectedChannelChanged -= _model_SelectedChannelChanged;
            _model.SelectedMethodChanged -= ModelSelectedMethodChanged;
            base.Cleanup();
        }

        #endregion
    }
}