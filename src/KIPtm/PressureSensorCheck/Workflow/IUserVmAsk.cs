using System;
using System.Threading;

namespace PressureSensorCheck.Workflow
{
    /// <summary>
    /// Визуальная модель для реализации запросов пользователю
    /// </summary>
    public interface IUserVmAsk
    {
        /// <summary>
        /// Пояснение
        /// </summary>
        string Note { get; set; }

        /// <summary>
        /// Выполняется запрос с подтвержнелием
        /// </summary>
        bool IsAsk { get; set; }

        /// <summary>
        /// Установить действие на подтверждение 
        /// </summary>
        /// <param name="accept"></param>
        void SetAcceptAction(Action accept);

        /// <summary>
        /// Сбросить действие на подтверждение
        /// </summary>
        void ResetSetAcceptAction();

        /// <summary>
        /// Вызов модального окна
        /// </summary>
        /// <param name="title">Заголовок</param>
        /// <param name="msg">Сообщение</param>
        /// <param name="cancel">Отмена</param>
        void AskModal(string title, string msg, CancellationToken cancel);
    }
}