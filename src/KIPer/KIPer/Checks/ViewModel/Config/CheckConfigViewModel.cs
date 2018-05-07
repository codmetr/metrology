using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ArchiveData.DTO;
using CheckFrame.ViewModel.Checks;
using KipTM.Checks;
using KipTM.Checks.ViewModel.Config;
using KipTM.Interfaces.Channels;

namespace KipTM.ViewModel.Checks.Config
{
    /// <summary>
    /// Визуальная модель шага конфигурации системы
    /// </summary>
    public class CheckConfigViewModel : INotifyPropertyChanged
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
                elV => new EthalonConfigViewModel(echalon.Values.FirstOrDefault(), _model.GetAvailableEthalons(elV),
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
            OnPropertyChanged("SelectedCheckType");
            OnPropertyChanged("SelectedChannel");
            OnPropertyChanged("CheckDeviceChanel");
        }

        /// <summary>
        /// Изменение измерительного канала связи с эталоном
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _model_SelectedChannelChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("SelectedChannel");
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
                OnPropertyChanged("EthalonOneCh");
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
                _customSetiings = value;
                OnPropertyChanged();
                IsCustomSettingsAvailable = _customSetiings != null;
            }
        }

        /// <summary>
        /// Наличие настройки конкретной методики
        /// </summary>
        public bool IsCustomSettingsAvailable
        {
            get { return _isCustomSettingsAvailable; }
            set { _isCustomSettingsAvailable = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Разрушение, отчистка ресурсов

        public virtual void Cleanup()
        {
            _checkDeviceChanel.ChannelTypeChanget -= _checkDeviceChanel_ChannelTypeChanget;
            _model.SelectedChannelChanged -= _model_SelectedChannelChanged;
            _model.SelectedMethodChanged -= ModelSelectedMethodChanged;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}