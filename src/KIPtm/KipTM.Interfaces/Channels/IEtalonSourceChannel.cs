using System.Threading;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces.Channels
{
    /// <summary>
    /// Эталонный канал-источник
    /// </summary>
    public interface IEtalonSourceChannel
    {
        bool Activate(ITransportChannelType transport);

        void Stop();

        bool SetEtalonValue(double aim, CancellationToken cancel);
    }

    /// <summary>
    /// Эталонный канал-источник
    /// </summary>
    /// <typeparam name="T">Единицы измерения</typeparam>
    public interface IEtalonSourceChannel<in T>:IEtalonSourceChannel
    {
        bool SetEtalonValue(double aim, T unit, CancellationToken cancel);
    }
}
