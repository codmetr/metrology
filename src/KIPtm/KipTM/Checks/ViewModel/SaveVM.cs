using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArchiveData;
using KipTM.EventAggregator;
using KipTM.Model.Checks;
using Tools.View;

namespace KipTM.Checks.ViewModel
{
    public class SaveVM:INotifyPropertyChanged, ISubscriber<EventArgRunState>
    {
        private readonly IEventAggregator _agregator;
        private bool _isSaveAvailable = true;
        private bool _isNotRun = true;
        private bool _commonAvailable = true;

        public SaveVM(IEventAggregator agregator)
        {
            _agregator = agregator;
            _agregator.Subscribe(this);
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        public ICommand Save { get { return new CommandWrapper(DoCommand);} }

        /// <summary>
        /// Доступность команды сохранения
        /// </summary>
        public bool IsSaveAvailable
        {
            get { return _isSaveAvailable; }
            set
            {
                if(value == _isSaveAvailable)
                    return;
                _isSaveAvailable = value;
                OnPropertyChanged("IsSaveAvailable");
            }
        }

        /// <summary>
        /// Общая доступность сервиса сохнанения
        /// </summary>
        /// <param name="sate"></param>
        public void ComonAvailable(bool sate)
        {
            _commonAvailable = sate;
            UpdateSate();
        }

        private void DoCommand()
        {
            _agregator.Post(new EventArgSave());
        }

        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void OnEvent(EventArgRunState message)
        {
            _isNotRun = !message.State;
            UpdateSate();
        }

        private void UpdateSate()
        {
            IsSaveAvailable = _isNotRun && _commonAvailable;
        }
    }
}
