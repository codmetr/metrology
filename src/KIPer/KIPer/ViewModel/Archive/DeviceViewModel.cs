using GalaSoft.MvvmLight;

namespace KipTM.ViewModel
{
    /// <summary>
    /// Идентификатор конкретного устройства
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DeviceViewModel : ViewModelBase, IDeviceViewModel
    {
        private string _serialNumber;
        private IDeviceTypeDescriptor _deviceType;

        /// <summary>
        /// Initializes a new instance of the DeviceViewModel class.
        /// </summary>
        public DeviceViewModel()
        {
        }

        /// <summary>
        /// Модель прибора
        /// </summary>
        public IDeviceTypeDescriptor DeviceType
        {
            get { return _deviceType; }
            set { Set(ref _deviceType, value); }
        }

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { Set(ref _serialNumber, value); }
        }
    }
}