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

        public double GetError(double ethalon, double measured)
        {
            return Math.Abs(measured) - Math.Abs(ethalon);
        }

        public bool IsCorrect(double ethalon, double measured)
        {
            return Math.Abs(measured) - Math.Abs(ethalon) < TolleranceAbs + Math.Abs(ethalon)*TolleranceMultipl;
        }
    }
}
