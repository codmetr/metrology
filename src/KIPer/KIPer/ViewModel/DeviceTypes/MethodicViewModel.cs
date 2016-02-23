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
    public class MethodicViewModel : ViewModelBase, IMethodicViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MethodicViewModel class.
        /// </summary>
        public MethodicViewModel()
        {
        }

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
    }
}