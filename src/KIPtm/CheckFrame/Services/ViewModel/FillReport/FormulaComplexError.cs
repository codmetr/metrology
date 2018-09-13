using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Services.ViewModel.FillReport
{
    /// <summary>
    /// Комплексная погрешность
    /// </summary>
    public class FormulaComplexError : IFormulaDescriptor
    {
        public double TolleranceMultipl { get; set; }
        public double TolleranceAbs { get; set; }

        public string Name { get { return "Комплексная погрешность"; } }

        public double GetError(double etalon, double measured)
        {
            return Math.Abs(measured) - Math.Abs(etalon);
        }

        public bool IsCorrect(double etalon, double measured)
        {
            return Math.Abs(measured) - Math.Abs(etalon) < TolleranceAbs + Math.Abs(etalon)*TolleranceMultipl;
        }
    }
}
