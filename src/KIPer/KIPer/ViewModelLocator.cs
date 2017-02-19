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
using KipTM.IOC;
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
        public ViewModelLocator()
        {
            UnityContainer unityContainer = new UnityContainer();

            unityContainer = IOCConfig.Config(unityContainer);

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));

            //SimpleIoc.Default.Register<MainViewModel>();
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