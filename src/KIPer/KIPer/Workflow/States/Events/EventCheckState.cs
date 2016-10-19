using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KipTM.Workflow.States.Events
{
    public class EventCheckState
    {
        public EventCheckState(bool runned)
        {
            Runned = runned;
        }

        public bool Runned { get; private set; }
    }
}
