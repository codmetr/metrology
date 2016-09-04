using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkerService.Filler;

namespace KipTM.ViewModel.ResultFiller.ADTS
{
    public class FillerKeyAttribute:FillerAttribute
    {
        public FillerKeyAttribute(string checkKey, string stepKey)
            : base(new Tuple<string, string>(checkKey, stepKey))
        {}
    }
}
