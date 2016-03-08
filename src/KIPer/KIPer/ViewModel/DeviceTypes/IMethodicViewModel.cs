using System.Collections.Generic;
using KipTM.Model.Devices;

namespace KipTM.ViewModel
{
    /// <summary>
    /// Методика проверки
    /// </summary>
    public interface IMethodicViewModel
    {
        /// <summary>
        ///  Название методики
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Проверяемые точки
        /// </summary>
        IEnumerable<IParameterViewModel> Points { get; set; }

        /// <summary>
        /// Типы вычисляемых параметров и функции их вычисления
        /// </summary>
        IDictionary<IParameterViewModel, IFunctionDescriptor> CalculatedParameters { get; set; }

        /// <summary>
        /// Типы требуемых эталонных параметров
        /// </summary>
        IEnumerable<string> TypesEtalonParameters { get; set; }
    }
}