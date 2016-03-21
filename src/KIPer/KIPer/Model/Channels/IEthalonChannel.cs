using System.Threading;

namespace KipTM.Model.Channels
{
    public interface IEthalonChannel
    {
        double GetEthalonValue(double point, CancellationToken calcel);
    }
}
