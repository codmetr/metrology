using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace KIPer.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DeviceDescriptorViewModel : ViewModelBase
    {
        private IDeviceViewModel _device;
        private IEnumerable<string> _checkParameters;
        private IDictionary<string, IDeviceViewModel> _etalonsByParameters;

        /// <summary>
        /// Initializes a new instance of the DeviceDescriptorViewModel class.
        /// </summary>
        public DeviceDescriptorViewModel()
        {
        }

        public IDeviceViewModel Device
        {
            get { return _device; }
            set { Set(ref _device, value); }
        }

        public IEnumerable<string> CheckParameters
        {
            get { return _checkParameters; }
            set { Set(ref _checkParameters, value); }
        }

        public IDictionary<string, IDeviceViewModel> EtalonsByParameters
        {
            get { return _etalonsByParameters; }
            set { Set(ref _etalonsByParameters, value); }
        }

        private IDictionary<string, IEnumerable<IParameterViewModel>> Methodics { get; set; }
    }
}