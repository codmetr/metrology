using System;
using System.Collections.Generic;
using System.Linq;
using ADTSChecks.Model.Checks;
using KipTM.Model.Checks;
using KipTM.ViewModel;
using MarkerService;

namespace ADTSChecks.ViewModel.ResultMarker.ADTS
{
    /// <summary>
    /// Генератор представления для точек проверки ADTS
    /// </summary>
    [Marker(typeof(ADTSTestMethod))]
    public class ADTSTestFabrik : IMarker<IParameterResultViewModel>
    {
        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный объект</param>
        /// <returns>описатель результата</returns>
        public IEnumerable<IParameterResultViewModel> Make(object target, IMarkerFabrik<IParameterResultViewModel> markerFabric)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (!(target is ADTSTestMethod)) throw new NoExpectedTypeParameterException(typeof(ADTSTestMethod), target.GetType());

            return Make((ADTSTestMethod)target, markerFabric);
        }

        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный типизированный объект</param>
        /// <returns>описатель результата</returns>
        private IEnumerable<IParameterResultViewModel> Make(ADTSTestMethod target, IMarkerFabrik<IParameterResultViewModel> markerFabric)
        {
            var result = target.Steps.Where(el=>el.Enabled).SelectMany(el => markerFabric.GetMarkers(el.Step.GetType(), el.Step)).ToList();
            return result;
        }

    }
}
