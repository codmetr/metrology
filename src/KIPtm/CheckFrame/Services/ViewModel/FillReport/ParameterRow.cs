using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Services.ViewModel.FillReport;

namespace KipTM.Services.ViewModel
{
    public class ParameterRow
    {
        /// <summary>
        /// Точка проверки
        /// </summary>
        public double Point { get; set; }
        /// <summary>
        /// Измеренное значение
        /// </summary>
        public double Measured { get; set; }
        /// <summary>
        /// Описатель вида погрешности
        /// </summary>
        public IFormulaDescriptor Formula { get; set; }
    }
}
