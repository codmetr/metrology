using System.Threading;
using CheckFrame.Model.TransportChannels;

namespace CheckFrame.Model.Channels
{
    public interface IEthalonChannel
    {
        bool Activate(ITransportChannelType transport);

        void Stop();

        double GetEthalonValue(double point, CancellationToken calcel);
    }
}
