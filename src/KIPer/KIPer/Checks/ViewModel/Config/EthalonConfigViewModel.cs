using System;
using System.Collections.Generic;
using ArchiveData.DTO;
using CheckFrame.Channels;
using GalaSoft.MvvmLight;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel;

namespace KipTM.Checks.ViewModel.Config
{
    public class EthalonConfigViewModel : ViewModelBase
    {
        private readonly DeviceWithChannel _ethalon;
        private readonly IDictionary<ChannelDescriptor, DeviceTypeDescriptor> _dicChannels;
        private SelectChannelViewModel _ethalonChanel;
        private bool _isAnalog;

        /// <summary>
        /// Визуальная модель конфигурации конкретного типа проверки
        /// </summary>
        public EthalonConfigViewModel(DeviceWithChannel ethalon, IDictionary<ChannelDescriptor, DeviceTypeDescriptor> dicChannels, IEnumerable<ITransportChannelType> channels)
        {
            _ethalon = ethalon;
            _dicChannels = dicChannels;
            _isAnalog = false;
            EthalonChanel = new SelectChannelViewModel(channels);
        }

        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IEnumerable<ChannelDescriptor> Channels { get { return _dicChannels.Keys; } }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public ChannelDescriptor SelectedEthalonType
        {
            get { return _ethalon.Channel; }
            set
            {
                _ethalon.Channel = value;
                _ethalon.Device.DeviceType = _dicChannels[value];
                IsAnalog = value.Key == UserEthalonChannel.Key;
                RaisePropertyChanged();
            }
        }

        public bool IsAnalog
        {
            get { return _isAnalog; }
            set
            {
                _isAnalog = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsNoAnalog");
            }
        }

        public bool IsNoAnalog
        {
            get { return !IsAnalog; }
        }

        /// <summary>
        /// Тип устройства аналогово эталона
        /// </summary>
        public string AnalogEthDevType
        {
            get { return _ethalon.Device.DeviceType.Model; }
            set
            {
                _ethalon.Device.DeviceType.Model = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Производитель эталона
        /// </summary>
        public string EthalonManufacturer
        {
            get { return _ethalon.Device.DeviceType.Manufacturer; }
            set
            {
                _ethalon.Device.DeviceType.Manufacturer = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string EthalonSerialNumber
        {
            get { return _ethalon.Device.SerialNumber; }
            set
            {
                _ethalon.Device.SerialNumber = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime EthalonPreviousCheckTime
        {
            get { return _ethalon.Device.PreviousCheckTime; }
            set
            {
                _ethalon.Device.PreviousCheckTime = value;
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
    }
}