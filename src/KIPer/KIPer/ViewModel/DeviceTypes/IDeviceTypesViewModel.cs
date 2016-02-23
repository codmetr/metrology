using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KipTM.ViewModel
{
    public interface IDeviceTypesViewModel
    {
        /// <summary>
        /// �������� ������ ����� ���������
        /// </summary>
        /// <param name="types">������ �������������� ����� ���������� ��������</param>
        void LoadTypes(IEnumerable<object> types);

        ObservableCollection<object> Types { get; set; }
        ICommand AddType { get; }
    }
}