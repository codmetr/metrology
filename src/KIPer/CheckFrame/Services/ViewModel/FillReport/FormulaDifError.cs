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

        public double GetError(double ethalon, double measured)
        {
            return (Math.Abs(measured) - Math.Abs(ethalon) )/ Math.Abs(ethalon);
        }

        public bool IsCorrect(double ethalon, double measured)
        {
            return (Math.Abs(measured) - Math.Abs(ethalon)) / Math.Abs(ethalon) < Tollerance;
        }
    }
}
