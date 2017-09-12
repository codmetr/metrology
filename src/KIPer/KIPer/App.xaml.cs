using System;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace KipTM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        #region Overrides of Application

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (logger != null)
                    logger.Info(string.Format("Start App"));
                base.OnStartup(e);
                Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            }
            catch (Exception ex)
            {
                if(logger!=null)
                    logger.Error(string.Format("StartUpError: {0}", ex.ToString()));
                throw;
            }

        }


        private static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.WriteLine(e.Exception.ToString());
        }

        #endregion
    }
}
