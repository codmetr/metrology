using System;
using System.Threading;
using ArchiveData.DTO;
using KipTM.Interfaces.Channels;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;

namespace CheckFrame.Channels
{
    public class UserEthalonChannel : IEthalonChannel
    {

        public static string Key = "Пользовательский канал";
        public static ChannelDescriptor Channel { get; }
            = new ChannelDescriptor(){
            Key = Key, Name = Key, Max = double.PositiveInfinity, Min = double.NegativeInfinity, Error = Double.Epsilon};
        public static DeviceTypeDescriptor Descriptor { get; }
            = new DeviceTypeDescriptor("Аналоговый прибор", "Приборы без аппаратного интерфейса", "") { TypeKey = Key};

        private readonly IUserChannel _userChannel;
        private readonly TimeSpan _waitPeriod;

        public UserEthalonChannel(IUserChannel userChannel, TimeSpan waitPeriod)
        {
            _userChannel = userChannel;
            _waitPeriod = waitPeriod;
        }

        public bool Activate(ITransportChannelType transport)
        {
            return true;
        }

        public void Stop()
        {
        }

        public double GetEthalonValue(double point, CancellationToken cancel)
        {
            var result = double.NaN;
            _userChannel.Message = string.Format("Укажите эталонное значение");
            _userChannel.RealValue = point;
            var wh = new ManualResetEvent(false);
            _userChannel.NeedQuery(UserQueryType.GetRealValue, wh);
            while (!wh.WaitOne(_waitPeriod))
            {
                if(cancel.IsCancellationRequested)
                    break;
            }
            if(cancel.IsCancellationRequested)
                return result;
            result = _userChannel.RealValue;
            return result;
        }
    }
}
