using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using KipTM.Interfaces;
using KipTM.Model;
using KipTM.Model.Devices;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SelectChannelViewModel : ViewModelBase
    {
        private ITransportChannelType _selectedChannel;
        private readonly IEnumerable<ITransportChannelType> _channels;
        private string _address;

        /// <summary>
        /// Initializes a new instance of the Pace5000ViewModel class.
        /// </summary>
        public SelectChannelViewModel()
        {
            SelectedChannel = new VisaChannelDescriptor();
            _channels = new[] { SelectedChannel };
        }

        public IEnumerable<ITransportChannelType> Channels
        {
            get { return _channels; }
        }

        public ITransportChannelType SelectedChannel
        {
            get { return _selectedChannel; }
            set { Set(ref _selectedChannel, value); }
        }

        public string Address
        {
            get { return _address; }
            set { Set(ref _address, value); }
        }

        public ITransportChannelType GetSelectedChannelType()
        {
            return SelectedChannel;
        }
    }
}