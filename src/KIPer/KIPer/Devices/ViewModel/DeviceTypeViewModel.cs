using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArchiveData.DTO;

namespace KipTM.ViewModel.DeviceTypes
{
    /// <summary>
    /// Комплексный описатель типа устройства
    /// </summary>
    public class DeviceTypeViewModel : INotifyPropertyChanged
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
            set { _device = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Доступные методики проверки
        /// </summary>
        public IEnumerable<IMethodicViewModel> Methodics
        {
            get { return _methodics; }
            set { _methodics = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранная методика
        /// </summary>
        public IMethodicViewModel SelectedMethodic
        {
            get { return _selectedMethodic; }
            set { _selectedMethodic = value;
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