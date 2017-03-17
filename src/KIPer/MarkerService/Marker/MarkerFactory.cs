using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tools;

namespace MarkerService
{
    public class MarkerFactory<T> : IMarkerFactory<T>
    {
        private static IMarkerFactory<T> _markerSolver = null;
        private readonly Dictionary<Type, IMarker<T>> _markers = new Dictionary<Type, IMarker<T>>();

        private MarkerFactory(){}

        #region Config
        /// <summary>
        /// Построить справочник маркеров из всех типов всех сборок
        /// </summary>
        private MarkerFactory<T> Config()
        {
            foreach (var assemblyType in TypeScaner.GetAllTypes())
            {// для каждой сборки
                ConfigType(assemblyType.Item1, assemblyType.Item2);
            }
            return this;
        }

        /// <summary>
        /// Построить справочник маркеров из типа
        /// </summary>
        private void ConfigType(Assembly assembly, Type type)
        {
            var intefaceMarkerType = typeof(IMarker<T>);
            if (!intefaceMarkerType.IsAssignableFrom(type) || intefaceMarkerType == type)
                return;
            IMarker<T> marker;
            try
            {
                marker = assembly.CreateInstance(type.FullName) as IMarker<T>;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error create instance type [{0}] asselbly[{1}] ", type.AssemblyQualifiedName, assembly.FullName), ex);
            }

            var attributes = type.GetCustomAttributes(typeof(MarkerAttribute), false).Select(el => el as MarkerAttribute);
            foreach (var attribute in attributes)
            {// для каждого атрибута
                if (_markers.ContainsKey(attribute.TargetType))
                    continue;
                _markers.Add(attribute.TargetType, marker);
            }
        }
        #endregion

        public static IMarkerFactory<T> Locator
        {
            get
            {
                return _markerSolver ?? (_markerSolver = (new MarkerFactory<T>()).Config());
            }
        }

        public static string СombineKeys(string keyBase, string key)
        {
            return string.Format("{0}.{1}", keyBase, key);
        }

        /// <summary>
        /// Получить маркер заданного элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<T> GetMarkers<Ttarget>(Ttarget item)
        {
            return GetMarkers(typeof(Ttarget), item);
        }

        /// <summary>
        /// Получить маркер заданного элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<T> GetMarkers(Type Ttarget, object item)
        {
            if (item == null)
                return new List<T>();
            if (!_markers.ContainsKey(Ttarget))
                return new List<T>();
            return _markers[Ttarget].Make(item, this);
        }
    }
}
