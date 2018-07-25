using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.Events
{
    /// <summary>
    /// Событие ошибки
    /// </summary>
    public class ErrorMessageEventArg
    {
        public ErrorMessageEventArg(string error)
        {
            Error = error;
        }

        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Error { get; private set; }
    }
}
