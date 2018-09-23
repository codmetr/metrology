using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PACESeries;
using Tools.View;

namespace PACESeriesUtil
{
    /// <summary>
    /// VM конфигурации подключения PACE
    /// </summary>
    public class PaceConfigViewModel : INotifyPropertyChanged
    {

        private ModelDescriptor _selectedModel;
        private int _address;
        private ChannelDiscriptor _selectedChannel;
        private IEnumerable<ChannelDiscriptor> _channels;
        private bool _isConnected;

        public PaceConfigViewModel()
        {
            Models = new List<ModelDescriptor>()
            {
                new ModelDescriptor(Model.PACE1000, "PACE 1000"/*TODO Локализовать*/),
                new ModelDescriptor(Model.PACE5000, "PACE 5000"/*TODO Локализовать*/),
                new ModelDescriptor(Model.PACE6000, "PACE 6000"/*TODO Локализовать*/)
            };
            _selectedModel = Models.FirstOrDefault();
            _channels = new List<ChannelDiscriptor>();
        }

        /// <summary>
        /// Набор доступных модификаций
        /// </summary>
        public IEnumerable<ModelDescriptor> Models { get; }

        /// <summary>
        /// Выбранная модификация
        /// </summary>
        public ModelDescriptor SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Адрес
        /// </summary>
        public int Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Каналы
        /// </summary>
        public IEnumerable<ChannelDiscriptor> Channels
        {
            get { return _channels; }
            set
            {
                _channels = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранный канал
        /// </summary>
        public ChannelDiscriptor SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                _selectedChannel = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Подключить/Отключить
        /// </summary>
        public ICommand ConnectDisconnect { get { return new CommandWrapper(DoConnectDisconnect);} }

        /// <summary>
        /// Подключить/отключить
        /// </summary>
        private void DoConnectDisconnect()
        {
            if (IsConnected)
                OnDisсonnect();
            else
                OnConnect();
        }

        /// <summary>
        /// Запрос подключения
        /// </summary>
        public event Action Connect;

        /// <summary>
        /// Запрос отключения
        /// </summary>
        public event Action Disсonnect;

        /// <summary>
        /// Подключено
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Вызвать команду "подключение"
        /// </summary>
        protected virtual void OnConnect()
        {
            Connect?.Invoke();
        }

        /// <summary>
        /// Вызвать команду "отключение"
        /// </summary>
        protected virtual void OnDisсonnect()
        {
            Disсonnect?.Invoke();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
