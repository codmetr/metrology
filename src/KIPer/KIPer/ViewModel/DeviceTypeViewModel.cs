using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace KIPer.ViewModel
{
    /// <summary>
    /// Комплексный описатель типа устройства
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DeviceTypeViewModel : ViewModelBase
    {
        private DeviceTypeDescriptor _device;
        private IEnumerable<string> _checkParameters;
        private IDictionary<string, DeviceTypeDescriptor> _etalonsByParameters;
        private IEnumerable<IMethodicViewModel> _methodics;

        /// <summary>
        /// Initializes a new instance of the DeviceDescriptorViewModel class.
        /// </summary>
        public DeviceTypeViewModel()
        {
        }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public DeviceTypeDescriptor Device
        {
            get { return _device; }
            set { Set(ref _device, value); }
        }

        /// <summary>
        /// Доступные методики проверки
        /// </summary>
        public IEnumerable<IMethodicViewModel> Methodics
        {
            get { return _methodics; }
            set { Set(ref _methodics, value); }
        }
    }
}