using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace KipTM.ViewModel
{
    /// <summary>
    /// Комплексный описатель типа устройства
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DeviceTypeViewModel : ViewModelBase
    {
        private IDeviceTypeDescriptor _device;
        private IEnumerable<IMethodicViewModel> _methodics;
        private IMethodicViewModel _selectedMethodic;

        /// <summary>
        /// Initializes a new instance of the DeviceDescriptorViewModel class.
        /// </summary>
        public DeviceTypeViewModel()
        {
        }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public IDeviceTypeDescriptor Device
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

        /// <summary>
        /// Выбранная методика
        /// </summary>
        public IMethodicViewModel SelectedMethodic
        {
            get { return _selectedMethodic; }
            set { Set(ref _selectedMethodic, value); }
        }
    }
}