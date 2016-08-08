using System;
using System.Threading;
using KipTM.Model.Checks;

namespace KipTM.Model.Channels
{
    /// <summary>
    /// Пользовательский интерфейс
    /// </summary>
    public interface IUserChannel
    {
        /// <summary>
        /// Тип ожидаемого действия пользователя
        /// </summary>
        UserQueryType QueryType { get; }

        /// <summary>
        /// Уточняющее сообщение для получения реального значения
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Эталонное значение от пользователя
        /// </summary>
        /// <remarks>Так же, при необходимости устанавливается в значение по умолчанию</remarks>
        bool AcceptValue { get; set; }

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
        void NeedQuey(UserQueryType queryType);

        /// <summary>
        /// Запрос на получение реального значение визуального параметра от пользователя
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="wh">Симофор по которому можно будет понять, что пользователь подтвердил ввод</param>
        void NeedQuery(UserQueryType queryType, EventWaitHandle wh);

        /// <summary>
        /// Поступил запрос на действие пользователя
        /// </summary>
        event EventHandler QueryStarted;
    }
}
