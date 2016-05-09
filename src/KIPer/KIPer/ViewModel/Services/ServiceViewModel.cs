using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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
        private IService _selectedService;
        private bool _isCanConnect;
        private bool _isCanNoConnect;
        private IService _showedService;

        /// <summary>
        /// Initializes a new instance of the ServiceViewModel class.
        /// </summary>
        public ServiceViewModel(IEnumerable<IService> services)
        {
            Services = services;
            SelectedService = Services.FirstOrDefault();
            Channel = new SelectChannelViewModel();
            ShowedService = null;
            IsCanConnect = true;
            IsCanNotConnect = false;
        }

        public IEnumerable<IService> Services { get; private set; }

        public IService SelectedService
        {
            get { return _selectedService; }
            set { Set(ref _selectedService, value); }
        }

        public IService ShowedService
        {
            get { return _showedService; }
            set { Set(ref _showedService, value); }
        }

        public SelectChannelViewModel Channel { get; private set; }

        public ICommand Connect { get { return new CommandWrapper(_connect); } }

        public ICommand Disconnect { get { return new CommandWrapper(_disconnect); } }

        public bool IsCanConnect
        {
            get { return _isCanConnect; }
            set { Set(ref _isCanConnect, value); }
        }

        public bool IsCanNotConnect
        {
            get { return _isCanNoConnect; }
            set { Set(ref _isCanNoConnect, value); }
        }

        private void _connect()
        {
            IsCanConnect = false;
            IsCanNotConnect = true;

            ShowedService = SelectedService;
            ShowedService.Start(Channel.GetSelectedChannelType());

        }

        private void _disconnect()
        {
            IsCanConnect = true;
            IsCanNotConnect = false;

            ShowedService.Stop();
            ShowedService = null;
        }
    }
}