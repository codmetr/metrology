using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace KipTM.ViewModel.ResultFiller
{
    class ResultFillerFabrik
    {
        private static ResultFillerFabrik _fillersSolver = null;

        private Dictionary<Tuple<string, string>, IResultFiller> _fillers =
            new Dictionary<Tuple<string, string>, IResultFiller>();

        #region Config
        /// <summary>
        /// Построить справочник заполнятелей из всех типов всех сборок
        /// </summary>
        private ResultFillerFabrik Config()
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
            var intefaceMarkerType = typeof(IResultFiller);
            if (!intefaceMarkerType.IsAssignableFrom(type) || intefaceMarkerType == type)
                return;
            IResultFiller filler;
            try
            {
                filler = assembly.CreateInstance(type.FullName) as IResultFiller;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error create instance type [{0}] asselbly[{1}] ", type.AssemblyQualifiedName, assembly.FullName), ex);
            }

            var attributes = type.GetCustomAttributes(typeof(FillerAttribute), false).Select(el => el as FillerAttribute);
            foreach (var attribute in attributes)
            {// для каждого атрибута
                if (_fillers.ContainsKey(new Tuple<string, string>(attribute.TypeKey, attribute.ResultKey)))
                    continue;
                _fillers.Add(new Tuple<string, string>(attribute.TypeKey, attribute.ResultKey), filler);
            }
        }
        #endregion

        public static ResultFillerFabrik Locator
        {
            get
            {
                return _fillersSolver ?? (_fillersSolver = (new ResultFillerFabrik()).Config());
            }
        }

        public IParameterResultViewModel GetResult<Ttarget>(string typeKey, string resultKey, Ttarget result)
        {
            return GetResult(typeof(Ttarget), typeKey, resultKey, result);
        }

        public IParameterResultViewModel GetResult(Type Ttarget, string typeKey, string resultKey, object item)
        {
            if (item == null)
                return null;
            var key = new Tuple<string, string>(typeKey, resultKey);
            if (!_fillers.ContainsKey(key))
                return null;
            return _fillers[key].GetFillResultMarker(item);
        }
    }
}
