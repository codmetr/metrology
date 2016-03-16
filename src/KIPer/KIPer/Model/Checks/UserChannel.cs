using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    /// <summary>
    /// Запрос на получение эталонного значение от пользователя
    /// </summary>
    internal class UserChannel : IUserChannel
    {
        private EventWaitHandle _wh = null;
        private bool _agreeValue;

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
        public void NeedQuey()
        {
            NeedQuery(null);
        }

        /// <summary>
        /// Запрос на получение эталонного значение параметра от пользователя
        /// </summary>
        /// <param name="wh">Симофор по которому можно будет понять, что пользователь подтвердил ввод</param>
        public void NeedQuery(EventWaitHandle wh)
        {
            _wh = wh;
            AgreeValue = false;
        }

    }
}
