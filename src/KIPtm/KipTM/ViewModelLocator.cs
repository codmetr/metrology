﻿/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:KipTM.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using KipTM.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System;
using KipTM.IOC;
using Microsoft.Practices.Unity;

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

            ServiceLocator.SetLocatorProvider(() => new KipTM.IOC.UnityServiceLocator(unityContainer));

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
            var dsdisp = ds as IDisposable;
            if(dsdisp != null)
                dsdisp.Dispose();
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
        }
    }
}