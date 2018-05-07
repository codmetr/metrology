using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CheckFrame.ViewModel.Archive;
using KipTM.Model.Devices;
using KipTM.ViewModel.DeviceTypes;

namespace KipTM.ViewModel
{
    public class MethodicViewModel : INotifyPropertyChanged, IMethodicViewModel
    {
        /// <summary>
        ///  Название методики
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Проверяемые точки
        /// </summary>
        public IEnumerable<IParameterViewModel> Points { get; set; }

        /// <summary>
        /// Типы вычисляемых параметров и функции их вычисления
        /// </summary>
        public IDictionary<IParameterViewModel, IFunctionDescriptor> CalculatedParameters { get; set; }
 
        /// <summary>
        /// типы требуемых эталонных параметров
        /// </summary>
        public IEnumerable<string> TypesEtalonParameters { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}