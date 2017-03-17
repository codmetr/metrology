using System;
using System.Collections.Generic;
using System.Linq;
using ADTSChecks.Model.Checks;
using CheckFrame.ViewModel.Archive;
using KipTM.ViewModel;
using MarkerService;

namespace ADTSChecks.ViewModel.ResultMarker.ADTS
{
    /// <summary>
    /// Генератор представления для точек проверки ADTS
    /// </summary>
    [Marker(typeof(Calibration))]
    public class ADTSCheckFactory : IMarker<IParameterResultViewModel>
    {
        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный объект</param>
        /// <returns>описатель результата</returns>
        public IEnumerable<IParameterResultViewModel> Make(object target, IMarkerFactory<IParameterResultViewModel> markerFactory)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (!(target is Calibration)) throw new NoExpectedTypeParameterException(typeof(Calibration), target.GetType());

            return Make((Calibration)target, markerFactory);
        }

        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный типизированный объект</param>
        /// <returns>описатель результата</returns>
        private IEnumerable<IParameterResultViewModel> Make(Calibration target, IMarkerFactory<IParameterResultViewModel> markerFactory)
        {
            var result = target.Steps.Where(el=>el.Enabled).SelectMany(el => markerFactory.GetMarkers(el.Step.GetType(), el.Step)).ToList();
            return result;
        }

    }
    ///// <summary>
    ///// Генератор представления для точек проверки ADTS
    ///// </summary>
    //[Marker(typeof(ADTSCheckMethod))]
    //public class ADTSCheckFactory : IMarker<IParameterResultViewModel>
    //{
    //    /// <summary>
    //    /// Получить описатель результата для заданного объекта
    //    /// </summary>
    //    /// <param name="target">заданный объект</param>
    //    /// <returns>описатель результата</returns>
    //    public IEnumerable<IParameterResultViewModel> Make(object target, IMarkerFactory<IParameterResultViewModel> markerFactory)
    //    {
    //        if (target == null) throw new ArgumentNullException("target");
    //        if (!(target is ADTSCheckMethod)) throw new NoExpectedTypeParameterException(typeof(ADTSCheckMethod), target.GetType());

    //        return Make((ADTSCheckMethod)target, markerFactory);
    //    }

    //    /// <summary>
    //    /// Получить описатель результата для заданного объекта
    //    /// </summary>
    //    /// <param name="target">заданный типизированный объект</param>
    //    /// <returns>описатель результата</returns>
    //    private IEnumerable<IParameterResultViewModel> Make(ADTSCheckMethod target, IMarkerFactory<IParameterResultViewModel> markerFactory)
    //    {
    //        var result = target.Steps.Where(el=>el.Enabled).SelectMany(el => markerFactory.GetMarkers(el.Step.GetType(), el.Step)).ToList();
    //        return result;
    //    }

    //}
}
