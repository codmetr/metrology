using System;
using MarkerService.Filler;

namespace ADTSChecks.ViewModel.ResultFiller.ADTS
{
    public class FillerKeyAttribute:FillerAttribute
    {
        public FillerKeyAttribute(string checkKey, string stepKey)
            : base(new Tuple<string, string>(checkKey, stepKey))
        {}
    }
}
