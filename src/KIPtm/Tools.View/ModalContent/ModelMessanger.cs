using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace Tools.View.ModalContent
{
    /// <summary>
    /// Описатель состояния модального окна
    /// </summary>
    public class ModalState : INotifyPropertyChanged
    {
        private bool _isShowModal = false;
        private string _note = string.Empty;
        private object _currentModal;
        private NoteMsg _noteMsg = new NoteMsg();

        public ModalState()
        {
            _currentModal = _noteMsg;
        }

        /// <summary>
        /// Показывать модальное окно
        /// </summary>
        public bool IsShowModal
        {
            get { return _isShowModal; }
            set
            {
                _isShowModal = value;
                OnPropertyChanged("IsShowModal");
            }
        }

        /// <summary>
        /// Показать сообщение, если модальный режим разрешен <see cref="IsShowModal"/>
        /// </summary>
        public void ShowNote(string msg)
        {
            _noteMsg.Note = msg;
            CurrentModal = _noteMsg;
        }

        /// <summary>
        /// Спросить у пользователя подтверждения
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="wh"></param>
        public void Ask(string msg, EventWaitHandle wh)
        {
            var ask = new AskViewModel(wh) {Queston = msg};
            CurrentModal = ask;
        }


        /// <summary>
        /// Спросить у пользователя подтверждения
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="wh"></param>
        public IDisposable AskModal(string msg, EventWaitHandle wh)
        {
            Ask(msg, wh);
            return ModalAsk.Show(this);
        }

        /// <summary>
        /// Спросить у пользователя подтверждения
        /// </summary>
        /// <param name="msg"></param>
        public IAsk AskOkCancel(string msg, EventWaitHandle wh)
        {
            var ask = new AskOkCancelViewModel(wh){Queston = msg};
            CurrentModal = ask;
            return ask;
        }

        /// <summary>
        /// Теущее модальное представление
        /// </summary>
        public object CurrentModal
        {
            get { return _currentModal; }
            set
            {
                _currentModal = value;
                OnPropertyChanged("CurrentModal");
            }
        }

        private class ModalAsk:IDisposable
        {
            private ModalState _state;

            private ModalAsk(ModalState state)
            {
                _state = state;
            }

            public static IDisposable Show(ModalState state)
            {
                var disp = new ModalAsk(state);
                state.IsShowModal = true;
                return disp;
            }

            public void Dispose()
            {
                _state.IsShowModal = false;
            }
        }

        #region INotifyPropertyChanged 

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }


    /// <summary>
    /// Менеджер модального окна
    /// </summary>
    public class ModelMessanger : IDisposable
    {
        private ModalState _vm;
        private Dispatcher _dispatcher;

        private ModelMessanger(ModalState vm, Dispatcher dispatcher)
        {
            _vm = vm;
            _dispatcher = dispatcher;
        }

        private void DoShow()
        {
            _dispatcher.Invoke(new Action(() =>{_vm.IsShowModal = true;}));
        }

        public void SetMessage(string msg)
        {
            _dispatcher.Invoke(new Action(() => { _vm.ShowNote(msg); }));
        }

        public void Ask(string msg, EventWaitHandle wh)
        {
            _dispatcher.Invoke(new Action(() => { _vm.Ask(msg, wh); }));
        }

        public static ModelMessanger Show(ModalState vm, Dispatcher dispatcher)
        {
            var res = new ModelMessanger(vm, dispatcher);
            res.DoShow();
            return res;
        }

        public void Dispose()
        {
            _dispatcher.Invoke(new Action(() => { _vm.IsShowModal = false; }));
        }
    }
}
