using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.ViewModel.Events
{
    /// <summary>
    /// Пояснение
    /// </summary>
    public class HelpMessageEventArg
    {
        /// <summary>
        /// Пояснение
        /// </summary>
        /// <param name="message"></param>
        public HelpMessageEventArg(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Пояснение
        /// </summary>
        public string Message { get; private set; }
    }
}
