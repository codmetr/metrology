using System.Threading;
using KipTM.Interfaces;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;

namespace PressureSensorCheck.Channels
{
    /// <summary>
    /// Эталонный канал - источник давления на основе пользовательского интерфейса
    /// </summary>
    internal class UChPresSource : IEthalonSourceChannel<Units>
    {
        private readonly IUserChannel _userChannel;

        public UChPresSource(IUserChannel userChannel)
        {
            _userChannel = userChannel;
        }

        public bool SetEthalonValue(double aim, Units unit, CancellationToken cancel)
        {
            _userChannel.Message = $"Установите на эталонном источнике давления значение {aim} {unit}, задайте реальное значение давления в графе Pэт и нажмите \"Далее\"";
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            WaitHandle.WaitAny(new[] { wh, cancel.WaitHandle });
            return true;
        }

        public bool Activate(ITransportChannelType transport)
        {
            return true;
        }

        public void Stop()
        {
            
        }

        public bool SetEthalonValue(double aim, CancellationToken cancel)
        {
            _userChannel.Message = $"Установите на эталонном источнике давления значение {aim}, задайте реальное значение давления в графе Pэт и нажмите \"Далее\"";
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetAccept, wh);
            WaitHandle.WaitAny(new[] { wh, cancel.WaitHandle });
            return true;
        }
    }
}