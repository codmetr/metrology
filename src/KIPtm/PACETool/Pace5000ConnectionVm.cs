using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PACETool
{
    class Pace5000ConnectionVm:BaseVm
    {
        private ConnectionPannelVm _connection;
        private Pace5000Vm _pace;

        public Pace5000ConnectionVm(Dispatcher disp) : base(disp)
        {
            _connection = new ConnectionPannelVm(disp);
            _pace = new Pace5000Vm(disp);
        }
        
        public ConnectionPannelVm Connection
        {
            get { return _connection; }
        }

        public Pace5000Vm Pace
        {
            get { return _pace; }
        }
    }
}
