using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTSData;
using KipTM.Model.Checks;
using MarkerService;
using KipTM.Model.Checks.Steps.ADTSTest;
using MarkerService.Filler;

namespace KipTM.ViewModel.ResultFiller.ADTS
{
    [FillerKey(ADTSTestMethod.Key, DoPointStep.KeyStep)]
    class ADTSTestPointFiller : IFiller<IParameterResultViewModel>
    {
        public IParameterResultViewModel FillMarker<T>(T result)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (!(result is AdtsPointResult)) throw new NoExpectedTypeParameterException(typeof(AdtsPointResult), result.GetType());

            return FillMarker(result as AdtsPointResult);
        }

        public IParameterResultViewModel FillMarker(object result)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (!(result is AdtsPointResult)) throw new NoExpectedTypeParameterException(typeof(AdtsPointResult), result.GetType());

            return FillMarker(result as AdtsPointResult);
        }

        public IParameterResultViewModel FillMarker(AdtsPointResult result)
        {
            return new ParameterResultViewModel()
            {
                NameParameter = string.Format("Поверка точки {0}", result.Point),
                Error = result.RealValue.ToString("F2"),
                PointMeashuring = result.Point.ToString("F2"),
                Tolerance = result.Tolerance.ToString("F2"),
                Unit = "kPa"
            };
        }
    }
}
