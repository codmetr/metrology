using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PACETool
{
    class BaseVm:INotifyPropertyChanged
    {
        private readonly Dispatcher _disp;

        public BaseVm(Dispatcher disp)
        {
            _disp = disp;
        }

        protected void Sync(Action act)
        {
            _disp.Invoke(act);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
