using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Tools.View.ModalContent
{
    /// <summary>
    /// Запрос пользователю
    /// </summary>
    public class AskViewModel:INotifyPropertyChanged
    {
        private string _queston;
        private readonly EventWaitHandle _wh;

        /// <summary>
        /// Запрос пользователю
        /// </summary>
        /// <param name="wh"></param>
        public AskViewModel(EventWaitHandle wh)
        {
            _wh = wh;
        }

        /// <summary>
        /// Запрос
        /// </summary>
        public string Queston
        {
            get { return _queston; }
            set
            {
                _queston = value;
                OnPropertyChanged("Queston");
            }
        }

        /// <summary>
        /// Подтверждение
        /// </summary>
        public ICommand Agree { get {return new CommandWrapper(()=> _wh.Set());} }

        #region INotifyPropertyChanged 

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
