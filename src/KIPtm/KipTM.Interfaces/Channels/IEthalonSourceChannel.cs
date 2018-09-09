using System.Threading;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces.Channels
{
    /// <summary>
    /// Эталонный канал-источник
    /// </summary>
    public interface IEthalonSourceChannel
    {
        bool Activate(ITransportChannelType transport);

        void Stop();

        bool SetEthalonValue(double aim, CancellationToken cancel);
    }

    /// <summary>
    /// Эталонный канал-источник
    /// </summary>
    /// <typeparam name="T">Единицы измерения</typeparam>
    public interface IEthalonSourceChannel<in T>:IEthalonSourceChannel
    {
        bool SetEthalonValue(double aim, T unit, CancellationToken cancel);
    }
}
