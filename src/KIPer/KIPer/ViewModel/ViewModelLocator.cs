/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:KipTM.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using CheckFrame.ViewModel.Archive;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using KipTM.Archive;
using KipTM.Checks.ViewModel.Config;
using KipTM.Interfaces;
using KipTM.Interfaces.Checks;
using KipTM.Settings;
using KipTM.ViewModel.ResultFiller;
using MarkerService;
using MarkerService.Filler;
using Microsoft.Practices.ServiceLocation;
using KipTM.Model;
using System;
using System.IO;
using System.Linq;
using KipTM.Design;
using KipTM.EventAggregator;
using KipTM.Interfaces.Archive;
using Microsoft.Practices.Unity;
using ReportService;
using SQLiteArchive;
using Tools;
using UnityServiceLocator = KipTM.IOC.UnityServiceLocator;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        //private IContainer ioc;
        public ViewModelLocator()
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs");
            var referencedPaths = Directory.GetFiles(path, "*.dll").ToList();
            var assemblies = new List<Assembly>();
            referencedPaths.ForEach(p => assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(p))));

            var pluginsTypes =
                assemblies.SelectMany(
                    el => el.GetExportedTypes());

            UnityContainer unityContainer = new UnityContainer();

            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));

            #region new config

            var cconfFactories = unityContainer.ResolveAll<ICustomConfigFactory>();

            var factoryDic = new Dictionary<Type, ICustomConfigFactory>();
            foreach (var factory in cconfFactories)
            {
                var atrs = factory.GetType().GetAttributes(typeof (CustomSettingsAttribute)).OfType<CustomSettingsAttribute>();
                foreach (var atr in atrs)
                {
                    if(!factoryDic.ContainsKey(atr.ArgumentType))
                        factoryDic.Add(atr.ArgumentType, factory);
                }
            }

            //unityContainer.RegisterTypes(pluginsTypes);
            if (ViewModelBase.IsInDesignModeStatic)
            {
                unityContainer.RegisterType<IDataService, DesignDataService>();
            }
            else
            {
                unityContainer.RegisterType<IDataService, Model.DataService>();
            }
            var devSettings = new List<IDeviceSettingsFactory>();
            var ethalonSettings = new List<IEthalonSettingsFactory>();
            var devTypeSettings = new List<IDeviceTypeSettingsFactory>();
            var methods = new List<IMethodFactory>();
            var services = new List<IService>();
            var features = new List<IFeaturesDescriptor>();
            var archives = new List<IArchiveDataDefault>();
            foreach (var type in pluginsTypes)
            {
                if (typeof (IDeviceSettingsFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType( typeof (IDeviceSettingsFactory), type, type.Name);
                if (typeof(IEthalonSettingsFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof(IEthalonSettingsFactory), type, type.Name);
                if (typeof(IDeviceTypeSettingsFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof(IDeviceTypeSettingsFactory), type, type.Name);
                if (typeof(IMethodFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof(IMethodFactory), type, type.Name);
                if (typeof(IService).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof(IService), type, type.Name);
                if (typeof(IFeaturesDescriptor).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof(IFeaturesDescriptor), type, type.Name);
                if (typeof(IArchiveDataDefault).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof(IArchiveDataDefault), type, type.Name);
                //if (typeof(IReportFabrik).IsSubclassOf(type))
                //    unityContainer.RegisterType(typeof(IReportFabrik), type);
            }

            unityContainer.RegisterType<IEventAggregator, EventAggregator.EventAggregator>();
            unityContainer.RegisterType<IArchive, ArchiveXML>();
            unityContainer.RegisterInstance<IEnumerable<IDeviceSettingsFactory>>(unityContainer.ResolveAll <IDeviceSettingsFactory>());
            unityContainer.RegisterInstance<IEnumerable<IEthalonSettingsFactory>>(unityContainer.ResolveAll<IEthalonSettingsFactory>());
            unityContainer.RegisterInstance<IEnumerable<IDeviceTypeSettingsFactory>>(unityContainer.ResolveAll<IDeviceTypeSettingsFactory>());
            unityContainer.RegisterInstance<IEnumerable<IMethodFactory>>(unityContainer.ResolveAll<IMethodFactory>());
            unityContainer.RegisterInstance<IEnumerable<IService>>(unityContainer.ResolveAll<IService>());
            unityContainer.RegisterInstance<IEnumerable<IFeaturesDescriptor>>(unityContainer.ResolveAll<IFeaturesDescriptor>());
            unityContainer.RegisterInstance<IEnumerable<IArchiveDataDefault>>(unityContainer.ResolveAll<IArchiveDataDefault>());

            //unityContainer.RegisterInstance<IEnumerable<IDeviceSettingsFactory>>(devSettings);
            //unityContainer.RegisterInstance<IEnumerable<IEthalonSettingsFactory>>(ethalonSettings);
            //unityContainer.RegisterInstance<IEnumerable<IDeviceTypeSettingsFactory>>(devTypeSettings);
            //unityContainer.RegisterInstance<IEnumerable<IMethodFactory>>(methods);
            //unityContainer.RegisterInstance<IEnumerable<IService>>(services);
            //unityContainer.RegisterInstance<IEnumerable<IFeaturesDescriptor>>(features);
            //unityContainer.RegisterInstance<IEnumerable<IArchiveDataDefault>>(archives);
            unityContainer.RegisterInstance<IMainSettings>(unityContainer.Resolve<IArchive>()
                    .Load(MainSettings.SettingsFileName, unityContainer.Resolve<MainSettingsFactory>().GetDefault()));
            unityContainer.RegisterType<IPropertiesLibrary, PropertiesLibrary>();
            //unityContainer.RegisterInstance<IPropertiesLibrary>(new PropertiesLibrary(
            //    unityContainer.ResolveAll<IArchiveDataDefault>(),
            //    unityContainer.ResolveAll<IFeaturesDescriptor>()));
            unityContainer.RegisterInstance<IMarkerFabrik<IParameterResultViewModel>>(
                MarkerFabrik<IParameterResultViewModel>.Locator);
            unityContainer.RegisterInstance<IFillerFabrik<IParameterResultViewModel>>(
                FillerFabrik<IParameterResultViewModel>.Locator);
            unityContainer.RegisterInstance<IReportFabrik>(ReportFabrik.Locator);
            unityContainer.RegisterInstance<IDictionary<Type, ICustomConfigFactory>>(factoryDic);
            unityContainer.RegisterType<IMethodsService, MethodsService>();
            unityContainer.RegisterType<MainViewModel>();

            #endregion

            #region old config
            /*
            SimpleIoc.Default.Register<IEventAggregator, EventAggregator.EventAggregator>();
            SimpleIoc.Default.Register<IArchive, ArchiveXML>();
            SimpleIoc.Default.Register<IMainSettings>(
                () => ServiceLocator.Current.GetInstance<IArchive>()
                        .Load(MainSettings.SettingsFileName,
                        ServiceLocator.Current.GetInstance < MainSettingsFactory >().GetDefault()));
            SimpleIoc.Default.Register<IPropertiesLibrary, PropertiesLibrary>();
            SimpleIoc.Default.Register<IMarkerFabrik<IParameterResultViewModel>>(
                () => MarkerFabrik<IParameterResultViewModel>.Locator, false);
            SimpleIoc.Default.Register<IFillerFabrik<IParameterResultViewModel>>(
                () => FillerFabrik<IParameterResultViewModel>.Locator, false);
            SimpleIoc.Default.Register<IReportFabrik>(() => ReportFabrik.Locator, true);
            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, Model.DataService>();
            }
            SimpleIoc.Default.Register<IMethodsService, MethodsService>();
            */
            #endregion

            SimpleIoc.Default.Register<MainViewModel>();

        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            var ds = ServiceLocator.Current.GetInstance<IDataService>();
            ds.SaveSettings();
            var dsdisp = ds as IDisposable;
            if(dsdisp != null)
                dsdisp.Dispose();
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
        }
    }
}