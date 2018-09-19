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
    public class PaseConfigViewModel:INotifyPropertyChanged
    {
        public class ModelDescriptor
        {
            public readonly string Name;
            internal readonly PACESeries.Model Id;

            public ModelDescriptor(Model id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        private ModelDescriptor _selectedModel;
        private int _address;

        public PaseConfigViewModel()
        {
            Models = new List<ModelDescriptor>()
            {
                new ModelDescriptor(Model.PACE1000, "PACE 1000"/*TODO Локализовать*/),
                new ModelDescriptor(Model.PACE5000, "PACE 5000"/*TODO Локализовать*/),
                new ModelDescriptor(Model.PACE6000, "PACE 6000"/*TODO Локализовать*/)
            };
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
