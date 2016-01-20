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
    public class DeviceTypesViewModel : ViewModelBase
    {
        private ObservableCollection<DeviceTypeViewModel> _types;

        /// <summary>
        /// Initializes a new instance of the DeviceTypesViewModel class.
        /// </summary>
        public DeviceTypesViewModel()
        {
        }

        /// <summary>
        /// Загрузка базовой конфигурации набора тестов
        /// </summary>
        /// <param name="types"></param>
        public void LoadTests(IEnumerable<DeviceTypeViewModel> types)
        {
            Types = new ObservableCollection<DeviceTypeViewModel>(types);
        }

        public ObservableCollection<DeviceTypeViewModel> Types
        {
            get { return _types; }
            set { Set(ref _types, value); }
        }

        public ICommand AddType { get{return new RelayCommand(()=>Types.Add(new DeviceTypeViewModel()));} }
    }
}