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
    public class AskOkCancelViewModel:INotifyPropertyChanged, IAsk
    {
        private string _queston;
        private readonly EventWaitHandle _wh;
        private bool _isAgree;

        /// <summary>
        /// Запрос пользователю
        /// </summary>
        /// <param name="wh"></param>
        public AskOkCancelViewModel(EventWaitHandle wh)
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
        /// Результат от пользователя
        /// </summary>
        public bool IsAgree
        {
            get { return _isAgree; }
            set
            {
                _isAgree = value; 
                OnPropertyChanged("IsAgree");
            }
        }

        /// <summary>
        /// Подтверждение
        /// </summary>
        public ICommand Agree { get {return new CommandWrapper(() =>
        {
            IsAgree = true;
            _wh.Set();
        });} }

        /// <summary>
        /// Отказ
        /// </summary>
        public ICommand Deny { get { return new CommandWrapper(() =>
        {
            IsAgree = false;
            _wh.Set();
        }); } }

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
