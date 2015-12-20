using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MainLoop
{
    public class Loop
    {
        /// <summary>
        /// Коллекция разделяемых ресурсов
        /// </summary>
        /// <remarks>
        /// Такими ресурсами в цикле опроса могут быть порты
        /// </remarks>
        private IDictionary<string, LoopDescriptor> lockers;

        private IDictionary<string, CancellationTokenSource> cancelThreadCollection;
    }
}
