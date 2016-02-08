using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace KIPer.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class DeviceTypesViewModel : ViewModelBase, IDeviceTypesViewModel
    {
        private ObservableCollection<object> _types;

        /// <summary>
        /// Initializes a new instance of the DeviceTypesViewModel class.
        /// </summary>
        public DeviceTypesViewModel()
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
            set { Set(ref _types, value); }
        }

        public ICommand AddType { get{return new RelayCommand(()=>Types.Add(new DeviceTypeViewModel()));} }
    }
}