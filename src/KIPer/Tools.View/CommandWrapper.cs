using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KipTM.ViewModel
{
    public class CommandWrapper : ICommand
    {
        private readonly Action _act;
        private readonly Action<object> _actWithParam;

        public CommandWrapper(Action act)
        {
            _act = act;
        }

        public CommandWrapper(Action<object> act)
        {
            _actWithParam = act;
        }

        public bool CanExecute(object parameter)
        {
            return _act != null || _actWithParam != null;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_act != null)
            {
                _act();
                return;
            }
            if (_actWithParam != null)
            {
                _actWithParam(parameter);
                return;
            }
        }
    }
}
