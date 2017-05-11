using System;
using System.Collections.Generic;
using System.Linq;
using ADTS;
using ADTSChecks.Checks.Data;
using ADTSChecks.Model.Checks;
using CheckFrame.ViewModel.Archive;
using KipTM.ViewModel;
using MarkerService;

namespace ADTSChecks.ViewModel.ResultMarker.ADTS
{
    /// <summary>
    /// Генератор представления для точек проверки ADTS
    /// </summary>
    [Marker(typeof(ADTSParameters))]
    public class ADTSParametersFactory : IMarker<IParameterResultViewModel>
    {
        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный объект</param>
        /// <returns>описатель результата</returns>
        public IEnumerable<IParameterResultViewModel> Make(object target, IMarkerFactory<IParameterResultViewModel> markerFactory)
        {
            if (target == null) throw new ArgumentNullException("target");
            var parameters = target as ADTSParameters;
            if (parameters != null)
                return Make(parameters, markerFactory);
            throw new NoExpectedTypeParameterException(typeof(ADTSParameters), target.GetType());

            
        }

        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный типизированный объект</param>
        /// <returns>описатель результата</returns>
        private IEnumerable<IParameterResultViewModel> Make(ADTSParameters target, IMarkerFactory<IParameterResultViewModel> markerFactory)
        {
            var result = target.Points.Where(el=>el.IsAvailable).SelectMany(el => Make(el, target.Unit.ToStr())).ToList();
            return result;
        }

        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный типизированный объект</param>
        /// <returns>описатель результата</returns>
        private IEnumerable<IParameterResultViewModel> Make(ADTSPoint target, string unit)
        {
            var itemMarker = new ParameterResultViewModel()
            {
                NameParameter = string.Format("Поверка точки {0} {1}", target.Pressure, unit),
                Error = String.Empty,
                PointMeasuring = string.Format("{0} {1}", target.Pressure.ToString("F2"), unit),
                Tolerance = string.Format("±{0} {1}", target.Tolerance.ToString("F2"), unit),
                Unit = String.Empty,
            };
            var result = new List<IParameterResultViewModel> { itemMarker };
            return result;
        }
    }
}
