﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Interfaces.Channels;

namespace PressureSensorCheck.Channels
{
    public class DPI620PressChannelFactory : IEtalonCannelFactory
    {
        public IEtalonChannel GetChanel(object model)
        {
            return new Dpi620PressChannel();
        }

        public object GetChanelViewModel(IEtalonChannel channel)
        {
            throw new NotImplementedException();
        }

        public Type ModelType { get; }
    }
}
