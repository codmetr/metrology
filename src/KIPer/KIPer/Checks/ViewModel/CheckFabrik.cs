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
        private IDictionary<Type, ICheckModelFactory> _fatories;

        public CheckFabrik(IDeviceManager deviceManager, IPropertyPool propertyPool)
        {
            _deviceManager = deviceManager;
            _propertyPool = propertyPool;
            Load();
        }

        /// <summary>
        /// Фабрика модели представления методики
        /// </summary>
        /// <returns></returns>
        public IMethodViewModel GetViewModelFor(object method, CheckConfigData checkConfig, object customConfig,
            TestResult resultBox, ITransportChannelType checkDeviceChanel, ITransportChannelType ethalonChanel)
        {
            IMethodViewModel result = null;

            if (_fatories.ContainsKey(method.GetType()))
                result = _fatories[method.GetType()].GetViewModel(method, checkConfig, customConfig, resultBox,
                    checkDeviceChanel, ethalonChanel);
            
            return result;
        }

        #region Service

        /// <summary>
        /// Загрузить все доступные фабрики презенторов
        /// </summary>
        public void Load()
        {
            _fatories = GetFactories().ToDictionary(el => el.Item1, el => el.Item2);
        }

        /// <summary>
        /// Получить список фабрик 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Tuple<Type, ICheckModelFactory>> GetFactories()
        {
            var types = TypeScaner.GetAllTypes().Where(el => el.GetType().GetAttributes(typeof(ViewModelFactoryAttribute)).Any());
            foreach (var type in types)
            {
                if (typeof(ICheckModelFactory).IsAssignableFrom(type.Item2))
                    yield return new Tuple<Type, ICheckModelFactory>(type.Item2, type.Item1.CreateInstance(type.Item2.FullName, true, BindingFlags.Default, null,
                        new[] { (object)_deviceManager, (object)_propertyPool }, CultureInfo.InvariantCulture,
                        new object[0]) as ICheckModelFactory);
            }
        }

        #endregion
    }
}
