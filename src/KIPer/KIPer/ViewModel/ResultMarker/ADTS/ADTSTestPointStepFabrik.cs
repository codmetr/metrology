using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.Checks.Steps.ADTSTest;
using MarkerService;

namespace KipTM.ViewModel.Archive.ADTS
{
    /// <summary>
    /// Генератор представления для точек проверки ADTS
    /// </summary>
    [Marker(typeof(DoPointStep))]
    public class ADTSTestPointStepFabrik : IMarker<IParameterResultViewModel>
    {
        /// <summary>
        /// Получить описатель результата для заданного объекта
        /// </summary>
        /// <param name="target">заданный объект</param>
        /// <returns>описатель результата</returns>
        public IEnumerable<IParameterResultViewModel> Make(object target, IMarkerFabrik<IParameterResultViewModel> markerFabric)
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
            var itemMarker = new ParameterResultViewModel() { NameParameter = target.Name, Tolerance = target.Tolerance.ToString() };
            var result = new List<IParameterResultViewModel> { itemMarker };
            return result;
        }

    }
}
