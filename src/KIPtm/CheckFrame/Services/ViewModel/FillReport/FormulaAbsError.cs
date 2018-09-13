using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Services.ViewModel.FillReport
{
    /// <summary>
    /// Абсолютная погрешность
    /// </summary>
    public class FormulaAbsError : IFormulaDescriptor
    {
        public double Tollerance { get; set; }

        public string Name { get { return "Абсолютная погрешность"; } }

        public double GetError(double etalon, double measured)
        {
            return Math.Abs(measured) - Math.Abs(etalon);
        }

        public bool IsCorrect(double etalon, double measured)
        {
            return Math.Abs(measured) - Math.Abs(etalon) < Tollerance;
        }
    }
}
