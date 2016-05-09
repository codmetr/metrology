using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KipTM.Model.TransportChannels;

namespace KipTM.Interfaces
{
    public interface IService
    {
        string Title { get; }
        void Start(int address, ITransportChannelType channel);
        void Stop();
    }
}
