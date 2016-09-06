﻿using System.Threading;
using KipTM.Model.TransportChannels;

namespace KipTM.Model.Channels
{
    public interface IEthalonChannel
    {
        bool Activate(ITransportChannelType transport);

        void Stop();

        double GetEthalonValue(double point, CancellationToken calcel);
    }
}