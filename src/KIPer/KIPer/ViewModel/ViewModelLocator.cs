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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using KipTM.Archive;
using KipTM.Interfaces;
using KipTM.Settings;
using Microsoft.Practices.ServiceLocation;
using KipTM.Model;
using System;

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
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<SQLiteArchive.IArchive, SQLiteArchive.Archive>();
            SimpleIoc.Default.Register<MainSettings>(() => ServiceLocator.Current.GetInstance<SQLiteArchive.IArchive>().Load(MainSettings.SettingsFileName, MainSettings.GetDefault()));
            SimpleIoc.Default.Register<ArchiveService>();
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

            //SimpleIoc.Default.Register<IServiceProvider, ServiceProvider>();
            //ioc = new Container();
            //ioc.Configure((reg)=>reg.ForRequestedType<>());
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
        }
    }
}