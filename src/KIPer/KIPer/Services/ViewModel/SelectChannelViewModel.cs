﻿using System;
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

        /// <summary>
        /// Initializes a new instance of the SelectChannelViewModel class.
        /// </summary>
        public SelectChannelViewModel(IEnumerable<ITransportChannelType> channels)
        {
            _channels = channels;
            SelectedChannel = _channels.FirstOrDefault();
        }

        public IEnumerable<ITransportChannelType> Channels
        {
            get { return _channels; }
        }

        public ITransportChannelType SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                Set(ref _selectedChannel, value);
                OnChannelTypeChanget();
            }
        }

        public ITransportChannelType GetSelectedChannelType()
        {
            return SelectedChannel;
        }

        /// <summary>
        /// Тип канала связи
        /// </summary>
        public event EventHandler ChannelTypeChanget;
        /// <summary>
        /// Выбор типа канала связи
        /// </summary>
        protected virtual void OnChannelTypeChanget()
        {
            EventHandler handler = ChannelTypeChanget;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}