using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Interfaces.Exceptions
{
    /// <summary>
    /// Не поддерживаемый тип канала
    /// </summary>
    public class TranspotrTypeNotAvailableException : Exception
    {
        public TranspotrTypeNotAvailableException(string message) : base(message)
        {
        }
    }
}
