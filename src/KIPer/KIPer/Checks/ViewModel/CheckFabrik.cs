using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ArchiveData.DTO;
using CheckFrame.Checks;
using CheckFrame.Model;
using CheckFrame.Model.Channels;
using CheckFrame.ViewModel.Checks.Channels;
using KipTM.Archive;
using KipTM.Checks;
using KipTM.EventAggregator;
using KipTM.Interfaces.Checks;
using KipTM.Model;
using KipTM.Model.Channels;
using KipTM.Model.Checks;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;
using ReportService;
using Tools;
using ITransportChannelType = KipTM.Model.TransportChannels.ITransportChannelType;

namespace KipTM.ViewModel.Checks
{
    public class CheckFabrik : ICheckFabrik
    {
        private readonly IDeviceManager _deviceManager;
        private readonly IPropertyPool _propertyPool;
        private IDictionary<Type, ICheckViewModelFactory> _fatories;
        private readonly IEventAggregator _eventAggregator;


        public CheckFabrik(IDeviceManager deviceManager, IPropertyPool propertyPool, IEnumerable<ICheckViewModelFactory> factories, IEventAggregator eventAggregator)
        {
            _deviceManager = deviceManager;
            _propertyPool = propertyPool;
            _eventAggregator = eventAggregator;
            Load(factories);
            foreach (var factory in _fatories.Values)
            {
                factory.SetDeviceManager(_deviceManager).SetPropertyPool(_propertyPool);
            }
        }

        /// <summary>
        /// Фабрика модели представления методики
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor(object method, CheckConfigData checkConfig, object customConfig,
            TestResult resultBox, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            IMethodViewModel result = null;
            var targetType = method.GetType();
            var key = _fatories.Keys.FirstOrDefault(el => el == targetType || el.IsAssignableFrom(targetType));

            if (key != null)
            {
                result = _fatories[key].GetViewModel(method, checkConfig, customConfig, resultBox,
                    checkDeviceChanel, ethalonChanel);
                if (result!=null)
                    result.SetAggregator(_eventAggregator);
            }

            return result;
        }

        #region Service

        /// <summary>
        /// Загрузить все доступные фабрики презенторов
        /// </summary>
        private void Load()
        {
            _fatories = GetFactories().ToDictionary(el => el.Item1, el => el.Item2);
        }

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
        private IEnumerable<Tuple<Type, ICheckViewModelFactory>> GetFactories()
        {
            var types = TypeScaner.GetAllTypes().Where(el => el.GetType().GetAttributes(typeof(ViewModelFactoryAttribute)).Any());
            foreach (var type in types)
            {
                if (typeof(ICheckViewModelFactory).IsAssignableFrom(type.Item2))
                    yield return new Tuple<Type, ICheckViewModelFactory>(type.Item2, type.Item1.CreateInstance(type.Item2.FullName, true, BindingFlags.Default, null,
                        new[] { (object)_deviceManager, (object)_propertyPool }, CultureInfo.InvariantCulture,
                        new object[0]) as ICheckViewModelFactory);
            }
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
                if(attr==null)
                    continue;

                yield return new Tuple<Type, ICheckViewModelFactory>(attr.ModelType, item);
            }
        }

        #endregion
    }
}
