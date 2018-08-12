using System;

namespace KipTM.Model.Checks
{
    public class EventArgRunState:EventArgs
    {
        public EventArgRunState(bool state)
        {
            State = state;
        }

        public bool State { get; private set; }
    }
}
