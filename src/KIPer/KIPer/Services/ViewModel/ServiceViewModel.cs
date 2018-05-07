using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using KipTM.Interfaces;
using Tools.View;

namespace KipTM.ViewModel
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        private IService _selectedService;
        private bool _isCanConnect;
        private bool _isCanNoConnect;
        private IService _showedService;

        /// <summary>
        /// Initializes a new instance of the ServiceViewModel class.
        /// </summary>
        public ServiceViewModel(IEnumerable<IService> services, SelectChannelViewModel channel)
        {
            Services = services;
            SelectedService = Services.FirstOrDefault();
            _showedService = null;
            Channel = channel;
            ShowedService = null;
            IsCanConnect = true;
            IsCanNotConnect = false;
        }

        public IEnumerable<IService> Services { get; private set; }

        public IService SelectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                OnPropertyChanged();
                ShowedService = SelectedService;
            }
        }

        public IService ShowedService
        {
            get { return _showedService; }
            set { _showedService = value;
                OnPropertyChanged();
            }
        }

        public SelectChannelViewModel Channel { get; private set; }

        public ICommand Connect { get { return new CommandWrapper(_connect); } }

        public ICommand Disconnect { get { return new CommandWrapper(_disconnect); } }

        public bool IsCanConnect
        {
            get { return _isCanConnect; }
            set { _isCanConnect = value;
                OnPropertyChanged();
            }
        }

        public bool IsCanNotConnect
        {
            get { return _isCanNoConnect; }
            set { _isCanNoConnect = value;
                OnPropertyChanged();
            }
        }

        private void _connect()
        {
            IsCanConnect = false;
            IsCanNotConnect = true;

            //ShowedService = SelectedService;
            ShowedService.Start(Channel.GetSelectedChannelType());
        }

        private void _disconnect()
        {
            IsCanConnect = true;
            IsCanNotConnect = false;

            ShowedService.Stop();
            //ShowedService = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}