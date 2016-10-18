namespace KipTM.EventAggregator
{
    public interface ISubscriber<in TMessage>
    {
        void OnEvent(TMessage message);
    }
}
