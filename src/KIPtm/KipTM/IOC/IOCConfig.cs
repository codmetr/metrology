using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ArchiveData;
using ArchiveData.DTO;
using CheckFrame;
using CheckFrame.Checks;
using CheckFrame.ViewModel.Archive;
using KipTM.Checks.ViewModel.Config;
using KipTM.DataService;
using KipTM.Design;
using KipTM.EventAggregator;
using KipTM.Interfaces;
using KipTM.Interfaces.Archive;
using KipTM.Interfaces.Checks;
using KipTM.Manuals.ViewModel;
using KipTM.Model;
using KipTM.Settings;
using KipTM.ViewModel;
using KipTM.ViewModel.ResultFiller;
using KipTM.Workflow.States;
using MarkerService;
using MarkerService.Filler;
using Microsoft.Practices.Unity;
using ReportService;
using SQLiteArchive;
using Tools;

namespace KipTM.IOC
{
    internal static class IOCConfig
    {
        /// <summary>
        /// Конфигурация IOC
        /// </summary>
        /// <param name="unityContainer"></param>
        /// <returns></returns>
        public static UnityContainer Config(UnityContainer unityContainer)
        {
            //unityContainer.RegisterTypes(pluginsTypes);
            //if (false)
            //{
            //    unityContainer.RegisterType<IDataService, DesignDataService>();
            //}
            //else
            //{
                unityContainer.RegisterType<IDataService, Model.DataService>();
            //}

            RegistrePlugins(unityContainer);

            unityContainer.RegisterType<IEventAggregator, EventAggregator.EventAggregator>();
            unityContainer.RegisterType<IArchive, ArchiveXML>();
            unityContainer.RegisterType<IDocsFactory, DocsFactory>();
            unityContainer.RegisterType<IMethodsService, MethodsService>();
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IFeaturesDescriptor>());
            unityContainer.RegisterInstance<FeatureDescriptorsCombiner>(
                unityContainer.Resolve<FeatureDescriptorsCombiner>());
            unityContainer.RegisterInstance<IDeviceManager>(
                new DeviceManager(unityContainer.Resolve<FeatureDescriptorsCombiner>()));

            unityContainer.RegisterInstance(unityContainer.ResolveAll<IDeviceSettingsFactory>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IEtalonSettingsFactory>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IDeviceTypeSettingsFactory>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IMethodFactory>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IService>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IArchiveDataDefault>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<ICheckViewModelFactory>());


            unityContainer.RegisterInstance<IMainSettings>(unityContainer.Resolve<IArchive>()
                    .Load(MainSettings.SettingsFileName, unityContainer.Resolve<MainSettingsFactory>().GetDefault()));
            unityContainer.RegisterInstance<IMarkerFactory<IParameterResultViewModel>>(
                MarkerFactory<IParameterResultViewModel>.Locator);
            unityContainer.RegisterInstance<IFillerFactory<IParameterResultViewModel>>(
                FillerFactory<IParameterResultViewModel>.Locator);
            unityContainer.RegisterInstance<IReportFactory>(new ReportFactory(unityContainer.ResolveAll<IReporter>()));
            unityContainer.RegisterInstance<IDictionaryPool>(new DictionariesPool());
            unityContainer.RegisterInstance<IDataAccessor>(new DataAccessor(DataPool.Load(unityContainer.Resolve<IDictionaryPool>(),
                new Dictionary<string, Type>() /*TODO: Задать типы результатов*/,
                new Dictionary<string, Type>() /*TODO: Задать типы конфигураций*/)));

            #region CustomConfigFactories

            var cconfFactories = unityContainer.ResolveAll<ICustomConfigFactory>();
            var factoryDic = new Dictionary<Type, ICustomConfigFactory>();
            foreach (var factory in cconfFactories)
            {
                var atrs =
                    factory.GetType().GetAttributes(typeof(CustomSettingsAttribute)).OfType<CustomSettingsAttribute>();
                foreach (var atr in atrs)
                {
                    if (!factoryDic.ContainsKey(atr.ArgumentType))
                        factoryDic.Add(atr.ArgumentType, factory);
                }
            }
            unityContainer.RegisterInstance<IDictionary<Type, ICustomConfigFactory>>(factoryDic);

            #endregion

            /*
            FeatureDescriptorsCombiner features = unityContainer.Resolve<FeatureDescriptorsCombiner>();
            IMainSettings settings = unityContainer.Resolve<IMainSettings>();
            IEnumerable<IMethodFactory> services = unityContainer.Resolve<IEnumerable<IMethodFactory>>();
            IMethodsService methodicService = unityContainer.Resolve<IMethodsService>();
            IMarkerFactory<IParameterResultViewModel> resultMaker = unityContainer.Resolve<IMarkerFactory<IParameterResultViewModel>>();
            IDictionaryPool dict = unityContainer.Resolve<IDictionaryPool>();
            IFillerFactory<IParameterResultViewModel> filler = unityContainer.Resolve<IFillerFactory<IParameterResultViewModel>>();
            IEnumerable<ICheckViewModelFactory> factoriesViewModels = unityContainer.Resolve<IEnumerable<ICheckViewModelFactory>>();
            IDictionary<Type, ICustomConfigFactory> customFatories = unityContainer.Resolve<IDictionary<Type, ICustomConfigFactory>>();
            IDataAccessor archive = unityContainer.Resolve<IDataAccessor>();
            IEventAggregator eventAggregator = unityContainer.Resolve<IEventAggregator>();
            IReportFactory reportFactory = unityContainer.Resolve<IReportFactory>();
            IDeviceManager deviceManager = unityContainer.Resolve<IDeviceManager>();
             */



            // Устоновка Singletone CheckWorkflowFactory
            unityContainer.RegisterType<CheckWorkflowFactory>(new ContainerControlledLifetimeManager());
            // Устоновка Singletone MethodsService
            unityContainer.RegisterType<IMethodsService, MethodsService>();
            unityContainer.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());

            return unityContainer;
        }

        /// <summary>
        /// Регистрация плагинов
        /// </summary>
        /// <param name="unityContainer"></param>
        private static void RegistrePlugins(UnityContainer unityContainer)
        {
            var pluginsTypes = GetPluginsTypes();
            foreach (var type in pluginsTypes)
            {
                if (typeof (IDeviceSettingsFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof (IDeviceSettingsFactory), type, type.Name);
                if (typeof (IEtalonSettingsFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof (IEtalonSettingsFactory), type, type.Name);
                if (typeof (IDeviceTypeSettingsFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof (IDeviceTypeSettingsFactory), type, type.Name);
                if (typeof (IMethodFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof (IMethodFactory), type, type.Name);
                if (typeof (IService).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof (IService), type, type.Name);
                if (typeof (IFeaturesDescriptor).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof (IFeaturesDescriptor), type, type.Name);
                if (typeof (IArchiveDataDefault).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof (IArchiveDataDefault), type, type.Name);
                if (typeof (ICustomConfigFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof (ICustomConfigFactory), type, type.Name);
                if (typeof (ICheckViewModelFactory).IsAssignableFrom(type) &&
                    type.GetAttributes(typeof (ViewModelFactoryAttribute)).Any())
                    unityContainer.RegisterType(typeof (ICheckViewModelFactory), type, type.Name);
                if (typeof (IReporter).IsAssignableFrom(type) && type.GetAttributes(typeof (ReportAttribute)).Any())
                    unityContainer.RegisterType(typeof (IReporter), type, type.Name);
            }
        }

        /// <summary>
        /// Загрузка плагинов
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Type> GetPluginsTypes()
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs");
            var referencedPaths = Directory.GetFiles(path, "*.dll").ToList();
            var assemblies = new List<Assembly>();
            referencedPaths.ForEach(p => assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(p))));

            var pluginsTypes = assemblies.SelectMany(el => el.GetExportedTypes());
            return pluginsTypes;
        }
    }
}
