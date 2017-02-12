using System;
using System.Threading;
using ArchiveData.DTO;
using KipTM.Model.Channels;
using KipTM.Model.TransportChannels;

namespace CheckFrame.Model.Channels
{
    public class UserEthalonChannel : IEthalonChannel
    {
        public static string Key = "Пользовательский канал";
        private static DeviceTypeDescriptor _typeDescriptor = new DeviceTypeDescriptor("", "", "");

        private readonly IUserChannel _userChannel;
        private readonly TimeSpan _waitPeriod;

        public UserEthalonChannel(IUserChannel userChannel, TimeSpan waitPeriod)
        {
            _userChannel = userChannel;
            _waitPeriod = waitPeriod;
        }

        public static DeviceTypeDescriptor TypeDescriptor { get {return _typeDescriptor;} }

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
