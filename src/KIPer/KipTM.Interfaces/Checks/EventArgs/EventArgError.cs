using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Model.Checks
{
    public class EventArgError : EventArgs
    {
        public string ErrorString;
        public object Error;
    }
}
