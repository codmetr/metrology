using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Services.ViewModel.FillReport
{
    /// <summary>
    /// Относительная погрешность
    /// </summary>
    public class FormulaDifError : IFormulaDescriptor
    {
        public double Tollerance { get; set; }

        public string Name { get { return "Относительная погрешность"; } }

        public double GetError(double etalon, double measured)
        {
            return (Math.Abs(measured) - Math.Abs(etalon) )/ Math.Abs(etalon);
        }

        public bool IsCorrect(double etalon, double measured)
        {
            return (Math.Abs(measured) - Math.Abs(etalon)) / Math.Abs(etalon) < Tollerance;
        }
    }
}
