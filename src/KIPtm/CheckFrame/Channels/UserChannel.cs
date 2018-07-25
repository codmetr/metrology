using System;
using System.Threading;
using KipTM.Model.Channels;

namespace CheckFrame.Model.Channels
{
    /// <summary>
    /// Запрос на получение эталонного значение от пользователя
    /// </summary>
    public class UserChannel : IUserChannel
    {
        private EventWaitHandle _wh = null;
        private bool _agreeValue;
        private UserQueryType _queryType;

        /// <summary>
        /// Тип ожидаемого действия пользователя
        /// </summary>
        public UserQueryType QueryType{get { return _queryType; }}

        /// <summary>
        /// Уточняющее сообщение для получения эталонного значения
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Эталонное значение от пользователя
        /// </summary>
        /// <remarks>Так же, при необходимости устанавливается в значение по умолчанию</remarks>
        public double RealValue { get; set; }


        /// <summary>
        /// Эталонное значение от пользователя
        /// </summary>
        /// <remarks>Так же, при необходимости устанавливается в значение по умолчанию</remarks>
        public bool AcceptValue { get; set; }

        /// <summary>
        /// Значение, показывающее, что пользователь подтвердил введенное эталонное значение
        /// </summary>
        public bool AgreeValue
        {
            get { return _agreeValue; }
            set
            {
                _agreeValue = value;
                if (_agreeValue && _wh != null)
                    _wh.Set();
            }
        }

        /// <summary>
        /// Запрос на получение эталонного значение параметра от пользователя
        /// </summary>
        /// <param name="wh">Симофор по которому можно будет понять, что пользователь подтвердил ввод</param>
        public void NeedQuery(UserQueryType queryType, EventWaitHandle wh)
        {
            _queryType = queryType;
            _wh = wh;

            AgreeValue = false;
            AcceptValue = false;
            OnQueryStarted();
        }

        /// <summary>
        /// Показать модальное сообщение пользователю
        /// </summary>
        /// <param name="title">заголовок</param>
        /// <param name="msg">сообщение</param>
        /// <param name="cancel">отменятор</param>
        public void ShowModal(string title, string msg, CancellationToken cancel)
        {
            //_vm.AskModal(title, msg, cancel);
        }

        /// <summary>
        /// Поступил запрос на действие пользователя
        /// </summary>
        public event EventHandler QueryStarted;

        protected virtual void OnQueryStarted()
        {
            EventHandler handler = QueryStarted;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
