/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:KipTM.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System.Reflection;
using System.Windows;
using CheckFrame.ViewModel.Archive;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using KipTM.Archive;
using KipTM.Interfaces;
using KipTM.Settings;
using KipTM.ViewModel.ResultFiller;
using MarkerService;
using MarkerService.Filler;
using Microsoft.Practices.ServiceLocation;
using KipTM.Model;
using System;
using System.IO;
using System.Linq;
using KipTM.EventAggregator;
using Microsoft.Practices.Unity;
using ReportService;
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
            referencedPaths.ForEach(p => AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(p)));

            UnityContainer unityContainer = new UnityContainer();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
            
            unityContainer.Configure().

            SimpleIoc.Default.Register<IEventAggregator, EventAggregator.EventAggregator>();
            SimpleIoc.Default.Register<SQLiteArchive.IArchive, SQLiteArchive.ArchiveXML>();
            SimpleIoc.Default.Register<IMainSettings>(() => ServiceLocator.Current.GetInstance<SQLiteArchive.IArchive>().Load(MainSettings.SettingsFileName, MainSettings.GetDefault()));
            SimpleIoc.Default.Register<IPropertiesLibrary, PropertiesLibrary>();
            SimpleIoc.Default.Register<IMarkerFabrik<IParameterResultViewModel>>(() => MarkerFabrik<IParameterResultViewModel>.Locator, false);
            SimpleIoc.Default.Register<IFillerFabrik<IParameterResultViewModel>>(() => FillerFabrik<IParameterResultViewModel>.Locator, false);
            SimpleIoc.Default.Register<IReportFabrik>(() => ReportFabrik.Locator, true);
            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }
            SimpleIoc.Default.Register<IMethodsService, MethodsService>();

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