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
        public double Point { get; set; }

        public double Measured { get; set; }

        public IFormulaDescriptor Formula { get; set; }
    }
}
