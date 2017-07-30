using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace Tools.View
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

#pragma warning disable 0067
        //[SuppressMessage("Microsoft.Performance", "CA067:TheEventIsNeverUsed", Justification = "CanExecuteChanged need for implimentation ICommand")]
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

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
