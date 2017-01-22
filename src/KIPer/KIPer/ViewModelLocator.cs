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
using CheckFrame.Checks;
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
using CheckFrame;
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

            //#region new config

            //unityContainer.RegisterTypes(pluginsTypes);
            if (ViewModelBase.IsInDesignModeStatic)
            {
                unityContainer.RegisterType<IDataService, DesignDataService>();
            }
            else
            {
                unityContainer.RegisterType<IDataService, Model.DataService>();
            }
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
                if (typeof(ICustomConfigFactory).IsAssignableFrom(type))
                    unityContainer.RegisterType(typeof(ICustomConfigFactory), type, type.Name);
                if (typeof(ICheckViewModelFactory).IsAssignableFrom(type) && type.GetAttributes(typeof(ViewModelFactoryAttribute)).Any())
                    unityContainer.RegisterType(typeof(ICheckViewModelFactory), type, type.Name);
                if (typeof(IReporter).IsAssignableFrom(type) && type.GetAttributes(typeof(ReportAttribute)).Any())
                    unityContainer.RegisterType(typeof(IReporter), type, type.Name);
            }

            unityContainer.RegisterType<IEventAggregator, EventAggregator.EventAggregator>();
            unityContainer.RegisterType<IArchive, ArchiveXML>();
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IFeaturesDescriptor>());
            unityContainer.RegisterInstance<FeatureDescriptorsCombiner>(
                unityContainer.Resolve<FeatureDescriptorsCombiner>());
            unityContainer.RegisterInstance<IDeviceManager>(
                new DeviceManager(unityContainer.Resolve<FeatureDescriptorsCombiner>()));

            unityContainer.RegisterInstance(unityContainer.ResolveAll<IDeviceSettingsFactory>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IEthalonSettingsFactory>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IDeviceTypeSettingsFactory>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IMethodFactory>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IService>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<IArchiveDataDefault>());
            unityContainer.RegisterInstance(unityContainer.ResolveAll<ICheckViewModelFactory>());


            unityContainer.RegisterInstance<IMainSettings>(unityContainer.Resolve<IArchive>()
                    .Load(MainSettings.SettingsFileName, unityContainer.Resolve<MainSettingsFactory>().GetDefault()));
            unityContainer.RegisterType<IPropertiesLibrary, PropertiesLibrary>();
            unityContainer.RegisterInstance<IMarkerFabrik<IParameterResultViewModel>>(
                MarkerFabrik<IParameterResultViewModel>.Locator);
            unityContainer.RegisterInstance<IFillerFabrik<IParameterResultViewModel>>(
                FillerFabrik<IParameterResultViewModel>.Locator);
            unityContainer.RegisterInstance<IReportFabrik>(new ReportFabrik(unityContainer.ResolveAll<IReporter>()));

            #region CustomConfigFactories

            var cconfFactories = unityContainer.ResolveAll<ICustomConfigFactory>();
            var factoryDic = new Dictionary<Type, ICustomConfigFactory>();
            foreach (var factory in cconfFactories)
            {
                var atrs =
                    factory.GetType().GetAttributes(typeof (CustomSettingsAttribute)).OfType<CustomSettingsAttribute>();
                foreach (var atr in atrs)
                {
                    if (!factoryDic.ContainsKey(atr.ArgumentType))
                        factoryDic.Add(atr.ArgumentType, factory);
                }
            }
            unityContainer.RegisterInstance<IDictionary<Type, ICustomConfigFactory>>(factoryDic);

            #endregion

            unityContainer.RegisterType<IMethodsService, MethodsService>();
            unityContainer.RegisterType<MainViewModel>();
            unityContainer.RegisterInstance<MainViewModel>(unityContainer.Resolve<MainViewModel>());

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