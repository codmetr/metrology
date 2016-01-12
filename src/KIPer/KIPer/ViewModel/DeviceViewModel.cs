using GalaSoft.MvvmLight;

namespace KIPer.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DeviceViewModel : ViewModelBase, IDeviceViewModel
    {
        private string _deviceCommonType;
        private string _serialNumber;
        private string _deviceManufacturer;
        private string _model;

        /// <summary>
        /// Initializes a new instance of the DeviceViewModel class.
        /// </summary>
        public DeviceViewModel()
        {
        }

        /// <summary>
        /// Модель прибора
        /// </summary>
        public string Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }

        /// <summary>
        /// Класс устройств
        /// </summary>
        public string DeviceCommonType
        {
            get { return _deviceCommonType; }
            set { Set(ref _deviceCommonType, value); }
        }

        /// <summary>
        /// Изготовитель
        /// </summary>
        public string DeviceManufacturer
        {
            get { return _deviceManufacturer; }
            set { Set(ref _deviceManufacturer, value); }
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