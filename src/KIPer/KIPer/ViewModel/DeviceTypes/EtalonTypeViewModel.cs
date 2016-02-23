using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class EtalonTypeViewModel : ViewModelBase
    {
        private IDeviceTypeDescriptor _device;
        private IEnumerable<string> _typesEtalonParameters;

        /// <summary>
        /// Initializes a new instance of the EtalonDeviceTypeViewModel class.
        /// </summary>
        public EtalonTypeViewModel()
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
        /// Типы эталонных параметров
        /// </summary>
        public IEnumerable<string> TypesEtalonParameters
        {
            get { return _typesEtalonParameters; }
            set { Set(ref _typesEtalonParameters, value); }
        }
    }
}