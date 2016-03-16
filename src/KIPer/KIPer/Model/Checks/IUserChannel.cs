using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KipTM.Model.Checks
{
    interface IUserChannel
    {
        /// <summary>
        /// Уточняющее сообщение для получения реального значения
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Реальное значение полученное от пользователя
        /// </summary>
        /// <remarks>Так же, при необходимости устанавливается в значение по умолчанию</remarks>
        double RealValue { get; set; }

        /// <summary>
        /// Значение, показывающее, что пользователь подтвердил введенное реальное значение
        /// </summary>
        bool AgreeValue { get; set; }

        /// <summary>
        /// Запрос на получение реального значение визуального параметра от пользователя
        /// </summary>
        void NeedQuey();

        /// <summary>
        /// Запрос на получение реального значение визуального параметра от пользователя
        /// </summary>
        /// <param name="wh">Симофор по которому можно будет понять, что пользователь подтвердил ввод</param>
        void NeedQuery(EventWaitHandle wh);
    }
}
