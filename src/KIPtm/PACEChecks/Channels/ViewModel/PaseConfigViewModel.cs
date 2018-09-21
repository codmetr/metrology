using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PACEChecks.Devices;
using PACESeries;

namespace PACEChecks.Channels.ViewModel
{
    /// <summary>
    /// VM описания модели
    /// </summary>
    public class ModelDescriptor
    {
        public string Name { get; private set; }
        internal readonly PACESeries.Model Id;

        public ModelDescriptor(Model id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    /// <summary>
    /// VM конфигурации подключения PACE
    /// </summary>
    public class PaceConfigViewModel : INotifyPropertyChanged
    {

        private ModelDescriptor _selectedModel;
        private int _address;

        public PaceConfigViewModel()
        {
            Models = new List<ModelDescriptor>()
            {
                //new ModelDescriptor(Model.PACE1000, "PACE 1000"/*TODO Локализовать*/),
                //new ModelDescriptor(Model.PACE5000, "PACE 5000"/*TODO Локализовать*/),
                new ModelDescriptor(Model.PACE6000, "PACE 6000"/*TODO Локализовать*/)
            };
            _selectedModel = Models.FirstOrDefault();
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
