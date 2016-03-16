using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    class UserEchalonChannel : IEthalonChannel
    {
        private readonly IUserChannel _userChannel;
        private readonly TimeSpan _waitPeriod;

        public UserEchalonChannel(IUserChannel userChannel, TimeSpan waitPeriod)
        {
            _userChannel = userChannel;
            _waitPeriod = waitPeriod;
        }


        public double GetEthalonValue(double point, CancellationToken calcel)
        {
            var result = double.NaN;
            _userChannel.Message = string.Format("Укажите эталонное значение");
            _userChannel.RealValue = point;
            var wh = new AutoResetEvent(false);
            _userChannel.NeedQuery(wh);
            while (!wh.WaitOne(_waitPeriod))
            {
                if(calcel.IsCancellationRequested)
                    break;
            }
            if(calcel.IsCancellationRequested)
                return result;
            result = _userChannel.RealValue;
            return result;
        }
    }
}
