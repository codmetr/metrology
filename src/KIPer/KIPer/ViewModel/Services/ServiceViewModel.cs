using System.Collections.Generic;
using GalaSoft.MvvmLight;
using KipTM.Interfaces;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ServiceViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ServiceViewModel class.
        /// </summary>
        public ServiceViewModel(IEnumerable<IService> services)
        {
            Services = services;
        }

        public IEnumerable<IService> Services { get; private set; } 
    }
}