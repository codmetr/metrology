using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NLog;

namespace PACETool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Logger _logger;

        //static App():base()
        //{
        //    //DispatcherHelper.Initialize();
        //}

        #region Overrides of Application

        public new int Run()
        {
            int res;

            _logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (_logger != null)
                    _logger.Info(string.Format("\n***Start App***"));
                res = base.Run();
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.Error(string.Format("Error: {0}", ex.ToString()));
                throw;
            }
            finally
            {
                if (_logger != null)
                    _logger.Info(string.Format("***Stop App***"));

            }

            return res;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.Error(string.Format("StartUpError: {0}", ex.ToString()));
                throw;
            }

        }


        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_logger != null)
                _logger.Error(string.Format("UnhandledException: {0}", e.Exception.ToString()));
            Debug.WriteLine(e.Exception.ToString());
        }

        #endregion
    }
}
