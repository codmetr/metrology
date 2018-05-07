using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArchiveData.DTO;

namespace KipTM.ViewModel.DeviceTypes
{
    public class EtalonTypeViewModel : INotifyPropertyChanged
    {
        private IDeviceTypeDescriptor _device;
        private IEnumerable<string> _typesEtalonParameters;

        /// <summary>
        /// Тип устройства
        /// </summary>
        public IDeviceTypeDescriptor Device
        {
            get { return _device; }
            set { _device = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Типы эталонных параметров
        /// </summary>
        public IEnumerable<string> TypesEtalonParameters
        {
            get { return _typesEtalonParameters; }
            set { _typesEtalonParameters = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}