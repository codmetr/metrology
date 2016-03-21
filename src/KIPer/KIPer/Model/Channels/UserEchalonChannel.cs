﻿using System;
using System.Threading;
using KipTM.Model.Checks;

namespace KipTM.Model.Channels
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


        public double GetEthalonValue(double point, CancellationToken cancel)
        {
            var result = double.NaN;
            _userChannel.Message = string.Format("Укажите эталонное значение");
            _userChannel.RealValue = point;
            var wh = new AutoResetEvent(false);
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