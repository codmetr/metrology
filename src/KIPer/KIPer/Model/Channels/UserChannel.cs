using System;
using System.Threading;
using KipTM.Model.Checks;

namespace KipTM.Model.Channels
{
    /// <summary>
    /// Запрос на получение эталонного значение от пользователя
    /// </summary>
    internal class UserChannel : IUserChannel
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
        public void NeedQuey(UserQueryType queryType)
        {
            NeedQuery(queryType, null);
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
