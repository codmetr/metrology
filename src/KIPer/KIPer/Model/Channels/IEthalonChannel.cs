using System.Threading;

namespace KipTM.Model.Channels
{
    public interface IEthalonChannel
    {
        bool Activate();

        void Stop();

        double GetEthalonValue(double point, CancellationToken calcel);
    }
}
