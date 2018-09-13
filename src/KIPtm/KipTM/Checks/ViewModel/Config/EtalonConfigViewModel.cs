using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArchiveData.DTO;
using CheckFrame.Channels;
using KipTM.Model.TransportChannels;
using KipTM.ViewModel;

namespace KipTM.Checks.ViewModel.Config
{
    public class EtalonConfigViewModel : INotifyPropertyChanged
    {
        private readonly DeviceWithChannel _etalon;
        private readonly IDictionary<ChannelDescriptor, DeviceTypeDescriptor> _dicChannels;
        private SelectChannelViewModel _etalonChanel;
        private bool _isAnalog;

        /// <summary>
        /// Визуальная модель конфигурации конкретного типа проверки
        /// </summary>
        public EtalonConfigViewModel(DeviceWithChannel etalon, IDictionary<ChannelDescriptor, DeviceTypeDescriptor> dicChannels, IEnumerable<ITransportChannelType> channels)
        {
            _etalon = etalon;
            _dicChannels = dicChannels;
            _isAnalog = false;
            EtalonChanel = new SelectChannelViewModel(channels);
        }

        /// <summary>
        /// Доступные типы устройства
        /// </summary>
        public IEnumerable<ChannelDescriptor> Channels { get { return _dicChannels.Keys; } }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public ChannelDescriptor SelectedEtalonType
        {
            get { return _etalon.Channel; }
            set
            {
                _etalon.Channel = value;
                _etalon.Device.DeviceType = _dicChannels[value];
                IsAnalog = value.Key == UserEtalonChannel.Key;
                OnPropertyChanged();
            }
        }

        public bool IsAnalog
        {
            get { return _isAnalog; }
            set
            {
                _isAnalog = value;
                OnPropertyChanged();
                OnPropertyChanged("IsNoAnalog");
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
            get { return _etalon.Device.DeviceType.Model; }
            set
            {
                _etalon.Device.DeviceType.Model = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Производитель эталона
        /// </summary>
        public string EtalonManufacturer
        {
            get { return _etalon.Device.DeviceType.Manufacturer; }
            set
            {
                _etalon.Device.DeviceType.Manufacturer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string EtalonSerialNumber
        {
            get { return _etalon.Device.SerialNumber; }
            set
            {
                _etalon.Device.SerialNumber = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата предыдущей поверки/калибровки
        /// </summary>
        public DateTime EtalonPreviousCheckTime
        {
            get { return _etalon.Device.PreviousCheckTime; }
            set
            {
                _etalon.Device.PreviousCheckTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Настройки канала эталона
        /// </summary>
        public SelectChannelViewModel EtalonChanel
        {
            get { return _etalonChanel; }
            set { _etalonChanel = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}