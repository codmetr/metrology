using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CheckFrame.Checks;
using KipTM.Archive;
using KipTM.EventAggregator;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using Tools;

namespace KipTM.Checks.ViewModel
{
    /// <summary>
    /// Конфигуратор фабрики визуальной модели проверки
    /// </summary>
    public class CheckPool
    {
        private readonly IDeviceManager _deviceManager;
        private readonly IPropertyPool _propertyPool;
        private IDictionary<Type, ICheckViewModelFactory> _fatories;


        /// <summary>
        /// Конфигуратор фабрики визуальной модели проверки
        /// </summary>
        /// <param name="deviceManager"></param>
        /// <param name="propertyPool"></param>
        /// <param name="factories"></param>
        public CheckPool(IDeviceManager deviceManager, IPropertyPool propertyPool, IEnumerable<ICheckViewModelFactory> factories)
        {
            _deviceManager = deviceManager;
            _propertyPool = propertyPool;
            Load(factories);
            foreach (var factory in _fatories.Values)
            {
                factory.SetDeviceManager(_deviceManager).SetPropertyPool(_propertyPool);
            }
        }

        /// <summary>
        /// Получить фабрику визуальной модели проверки по типу устройства
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public ICheckViewModelFactory GetFactory(Type targetType)
        {
            ICheckViewModelFactory result = null;
            var key = _fatories.Keys.FirstOrDefault(el => el == targetType || el.IsAssignableFrom(targetType));

            if (key != null)
            {
                result = _fatories[key];
            }
            return result;
        }

        #region Service
        
        /// <summary>
        /// Загрузить все доступные фабрики презенторов
        /// </summary>
        private void Load(IEnumerable<ICheckViewModelFactory> factories)
        {
            _fatories = GetFactories(factories).ToDictionary(el => el.Item1, el => el.Item2);
        }
        
        /// <summary>
        /// Получить список фабрик 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Tuple<Type, ICheckViewModelFactory>> GetFactories(IEnumerable<ICheckViewModelFactory> factories)
        {
            foreach (ICheckViewModelFactory item in factories)
            {
                ViewModelFactoryAttribute attr = item.GetType().GetCustomAttributes<ViewModelFactoryAttribute>().FirstOrDefault();
                if (attr == null)
                    continue;

                yield return new Tuple<Type, ICheckViewModelFactory>(attr.ModelType, item);
            }
        }

        #endregion
    }
}
