using System.Threading;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces.Channels
{
    public interface IEtalonChannel
    {
        bool Activate(ITransportChannelType transport);

        void Stop();

        double GetEtalonValue(double point, CancellationToken cancel);
    }
}
