using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArchiveData.DTO;

namespace KipTM.ViewModel
{
    /// <summary>
    /// Идентификатор конкретного устройства
    /// </summary>
    public class DeviceViewModel : INotifyPropertyChanged, IDeviceViewModel
    {
        private string _serialNumber;
        private IDeviceTypeDescriptor _deviceType;
        private readonly DeviceDescriptor _device;

        /// <summary>
        /// Initializes a new instance of the DeviceViewModel class.
        /// </summary>
        public DeviceViewModel(DeviceDescriptor device)
        {
            _device = device;
            _deviceType = _device.DeviceType;
            _serialNumber = _device.SerialNumber;
        }

        /// <summary>
        /// Модель прибора
        /// </summary>
        public IDeviceTypeDescriptor DeviceType
        {
            get { return _deviceType; }
            set
            {
                if(value == _deviceType)
                    return;
                _deviceType = value;
                OnPropertyChanged("DeviceType");
            }
        }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                if(value == _serialNumber)
                    return;
                _serialNumber = value;
                OnPropertyChanged("SerialNumber");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}