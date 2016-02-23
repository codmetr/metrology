using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KipTM.ViewModel
{
    public interface IDeviceTypesViewModel
    {
        /// <summary>
        /// Загрузка набора типов устройств
        /// </summary>
        /// <param name="types">Набора поддерживаемых типов поверяемых приборов</param>
        void LoadTypes(IEnumerable<object> types);

        ObservableCollection<object> Types { get; set; }
        ICommand AddType { get; }
    }
}