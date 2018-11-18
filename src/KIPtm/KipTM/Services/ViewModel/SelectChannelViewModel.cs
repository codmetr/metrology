using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using KipTM.Model.TransportChannels;

namespace KipTM.ViewModel
{
    /// <summary>
    /// Выбор каналов
    /// </summary>
    public class SelectChannelViewModel : INotifyPropertyChanged
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
                _selectedChannel = value;
                OnPropertyChanged();
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}