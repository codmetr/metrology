using CheckFrame.Model.TransportChannels;

namespace CheckFrame.Interfaces
{
    public interface IService
    {
        string Title { get; }
        void Start(ITransportChannelType channel);
        void Stop();
    }
}
