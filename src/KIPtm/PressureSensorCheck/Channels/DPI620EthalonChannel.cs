﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KipTM.Interfaces.Channels;
using KipTM.Model.TransportChannels;

namespace PressureSensorCheck.Channels
{
    public class Dpi620PressChannel : IEtalonChannel
    {
        public bool Activate(ITransportChannelType transport)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public double GetEtalonValue(double point, CancellationToken calcel)
        {
            throw new NotImplementedException();
        }
    }
}
