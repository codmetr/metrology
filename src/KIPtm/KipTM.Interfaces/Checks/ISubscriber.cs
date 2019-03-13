namespace KipTM.EventAggregator
{
    /// <summary>
    /// Подписчик
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface ISubscriber<in TMessage>
    {
        /// <summary>
        /// Обработать событие
        /// </summary>
        /// <param name="message"></param>
        void OnEvent(TMessage message);
    }
}
