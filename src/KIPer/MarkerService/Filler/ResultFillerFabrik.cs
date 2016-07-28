using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MarkerService.Filler;
using Tools;

namespace KipTM.ViewModel.ResultFiller
{
    public class FillerFabrik<T> : IFillerFabrik<T>
    {
        public static FillerFabrik<T> _fillersSolver = null;

        private readonly Dictionary<object, IFiller<T>> _fillers =
            new Dictionary<object, IFiller<T>>();

        private FillerFabrik()
        {}

        #region Config
        /// <summary>
        /// Построить справочник заполнятелей из всех типов всех сборок
        /// </summary>
        private FillerFabrik<T> Config()
        {
            foreach (var assemblyType in TypeScaner.GetAllTypes())
            {// для каждой сборки
                ConfigType(assemblyType.Item1, assemblyType.Item2);
            }
            return this;
        }

        /// <summary>
        /// Построить справочник заполнятелей из типа
        /// </summary>
        private void ConfigType(Assembly assembly, Type type)
        {
            var intefaceMarkerType = typeof(IFiller<T>);
            if (!intefaceMarkerType.IsAssignableFrom(type) || intefaceMarkerType == type)
                return;
            IFiller<T> filler;
            try
            {
                filler = assembly.CreateInstance(type.FullName) as IFiller<T>;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error create instance type [{0}] asselbly[{1}] ", type.AssemblyQualifiedName, assembly.FullName), ex);
            }

            var attributes = type.GetCustomAttributes(typeof(FillerAttribute), true).Select(el => el as FillerAttribute);
            foreach (var attribute in attributes)
            {// для каждого атрибута
                if (_fillers.ContainsKey(attribute.Key))
                    continue;
                _fillers.Add(attribute.Key, filler);
            }
        }
        #endregion

        public static IFillerFabrik<T> Locator
        {
            get
            {
                return _fillersSolver ?? (_fillersSolver = (new FillerFabrik<T>()).Config());
            }
        }

        public T FillMarker<TTarget>(object Key, TTarget result)
        {
            return FillMarker(typeof(TTarget), Key, result);
        }

        public T FillMarker(Type ttarget, object Key, object item)
        {
            if (item == null)
                return default(T);
            var key = Key;
            if (!_fillers.ContainsKey(key))
                return default(T);
            return _fillers[key].FillMarker(item);
        }
    }
}
