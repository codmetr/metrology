using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using KipTM.ViewModel.DeviceTypes;
using Tools.View;

namespace KipTM.Devices.ViewModel
{
    public class DeviceTypeCollectionViewModel : INotifyPropertyChanged, IDeviceTypesViewModel
    {
        private ObservableCollection<object> _types;

        /// <summary>
        /// Initializes a new instance of the DeviceTypesViewModel class.
        /// </summary>
        public DeviceTypeCollectionViewModel()
        {
        }

        /// <summary>
        /// Загрузка набора типов устройств
        /// </summary>
        /// <param name="types">Набора поддерживаемых типов поверяемых приборов</param>
        public void LoadTypes(IEnumerable<object> types)
        {
            Types = new ObservableCollection<object>(types);
        }

        public ObservableCollection<object> Types
        {
            get { return _types; }
            set { _types = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddType { get{return new CommandWrapper(()=>Types.Add(new DeviceTypeViewModel()));} }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}