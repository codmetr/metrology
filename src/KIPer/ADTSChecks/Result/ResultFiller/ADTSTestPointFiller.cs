using System;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Steps.ADTSTest;
using ADTSData;
using CheckFrame.ViewModel.Archive;
using KipTM.ViewModel;
using MarkerService;
using MarkerService.Filler;

namespace ADTSChecks.ViewModel.ResultFiller.ADTS
{
    [FillerKey(Test.key, DoPointStep.KeyStep)]
    public class ADTSTestPointFiller : IFiller<IParameterResultViewModel>
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
                Unit = "мБар"
            };
        }
    }
}
