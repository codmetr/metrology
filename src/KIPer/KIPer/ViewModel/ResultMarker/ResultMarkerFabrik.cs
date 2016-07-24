using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.ResultMarker
{
    class ResultMarkerFabrik : IResultMarkerFabrik
    {
        private static IResultMarkerFabrik _markerSolver = null;
        private readonly Dictionary<Type, IResultMarker> _markers = new Dictionary<Type, IResultMarker>();

        #region Config
        /// <summary>
        /// Построить справочник маркеров из всех типов всех сборок
        /// </summary>
        private ResultMarkerFabrik Config()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {// для каждой сборки
                ConfigAssembly(assembly);
            }
            return this;
        }

        /// <summary>
        /// Построить справочник маркеров из всех типов сборки
        /// </summary>
        private void ConfigAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {// для каждого типа
                ConfigType(assembly, type);
            }
        }
        /// <summary>
        /// Построить справочник маркеров из типа
        /// </summary>
        private void ConfigType(Assembly assembly, Type type)
        {
            var intefaceMarkerType = typeof(IResultMarker);
            if (!intefaceMarkerType.IsAssignableFrom(type))
                return;
            IResultMarker marker;
            try
            {
                marker = assembly.CreateInstance(type.AssemblyQualifiedName) as IResultMarker;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error create instance type [{0}] asselbly[{1}] ", type.AssemblyQualifiedName, assembly.FullName), ex);
            }

            var attributes = type.GetCustomAttributes(typeof(ResultMarkerAttribute), false).Select(el => el as ResultMarkerAttribute);
            foreach (var attribute in attributes)
            {// для каждого атрибута
                if (_markers.ContainsKey(attribute.TargetType))
                    continue;
                _markers.Add(attribute.TargetType, marker);
            }
        }
        #endregion

        public static IResultMarkerFabrik Locator
        {
            get
            {
                return _markerSolver ?? (_markerSolver = (new ResultMarkerFabrik()).Config());
            }
        }

        public static string СombineKeys(string keyBase, string key)
        {
            return string.Format("{0}.{1}", keyBase, key);
        }

        /// <summary>
        /// Получить текстовое проедставление(маркер) заданного элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<IParameterResultViewModel> GetMarkers<T>(T item)
        {
            if (item == null)
                return null;
            if (!_markers.ContainsKey(typeof(T)))
                return null;
            return _markers[typeof(T)].Make(item);
        }
    }
}
