using System;
using ADTSChecks.Model.Checks;
using ADTSChecks.Model.Steps.ADTSCalibration;
using ADTSData;
using CheckFrame.ViewModel.Archive;
using KipTM.ViewModel;
using MarkerService;
using MarkerService.Filler;

namespace ADTSChecks.ViewModel.ResultFiller.ADTS
{
    [FillerKey(Calibration.key, DoPointStep.KeyStep)]
    public class ADTSCheckPointFiller : IFiller<IParameterResultViewModel>
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
                NameParameter = string.Format("Калибровка точки {0}", result.Point),
                Error = result.RealValue.ToString("F2"),
                PointMeasuring = string.Format("{0} {1}", result.Point.ToString("F2"), result.Unit),
                Tolerance = string.Format("±{0} {1}", result.Tolerance.ToString("F2"), result.Unit),
                Unit = result.Unit,
            };
        }
    }
}
