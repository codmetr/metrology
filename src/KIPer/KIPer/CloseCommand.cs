using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Tools.View;

namespace KipTM
{
    public class CloseCommand
    {
        public static readonly ICommand Command =
        new CommandWrapper(o => ((Window)o).Close());
    }
}
