namespace KipTM.EventAggregator
{
    /// <summary>
    /// Агрегатор событий
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Подписать
        /// </summary>
        /// <param name="subscriber">Подписчик</param>
        /// <param name="token">Идентификатор события</param>
        void Subscribe(object subscriber, object token = null);
        
        /// <summary>
        /// Отписать 
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="token"></param>
        void Unsubscribe(object subscriber, object token = null);

        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="token"></param>
        void Send<TMessage>(TMessage message, object token = null);

        /// <summary>
        /// Опубликовать асинхронно
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="token"></param>
        void Post<TMessage>(TMessage message, object token = null);
    }
}
