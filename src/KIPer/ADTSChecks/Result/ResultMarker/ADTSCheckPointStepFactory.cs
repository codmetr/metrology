using System;
using System.Collections.Generic;
using ADTSChecks.Model.Steps.ADTSCalibration;
using CheckFrame.ViewModel.Archive;
using KipTM.ViewModel;
using MarkerService;

namespace ADTSChecks.ViewModel.ResultMarker.ADTS
{
    /// <summary>
    /// Генератор представления для точек проверки ADTS
    /// </summary>
    [Marker(typeof(DoPointStep))]
    public class ADTSCheckPointStepFactory : IMarker<IParameterResultViewModel>
    {
        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный объект</param>
        /// <returns>описатель результата</returns>
        public IEnumerable<IParameterResultViewModel> Make(object target, IMarkerFactory<IParameterResultViewModel> markerFactory)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (!(target is DoPointStep)) throw new NoExpectedTypeParameterException(typeof(DoPointStep), target.GetType());

            return Make((DoPointStep)target);
        }

        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный типизированный объект</param>
        /// <returns>описатель результата</returns>
        private IEnumerable<IParameterResultViewModel> Make(DoPointStep target)
        {
            var itemMarker = new ParameterResultViewModel()
            {
                NameParameter = string.Format("Калибровка точки {0}", target.Point),
                PointMeashuring = target.Point.ToString("F2"),
                Tolerance = target.Tolerance.ToString("F2"),
                Error = "",
                Unit = "мБар"
            };
            var result = new List<IParameterResultViewModel> { itemMarker };
            return result;
        }

    }
}
