using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class ADTSParser
    {
        #region GetState
        public string GetCommandGetState()
        {
            const string cmd = "SOUR:STAT?";
            return cmd;
        }

        public State? ParseGetState(string message)
        {
            State? state;
            switch (message)
            {
                case "ON": state = State.Control; break;
                case "OFF": state = State.Measure; break;
                case "HOLD": state = State.Hold; break;
                default: state = null; break;
            }
            return state;
        }
        #endregion

        #region SetState
        public string GetCommandSetState(State state)
        {
            const string cmdFrame = "SOUR:STAT {0}";
            switch (state)
            {
                case State.Control:
                    return string.Format(cmdFrame, "ON");
                case State.Measure:
                    return string.Format(cmdFrame, "OFF");
                case State.Hold:
                    return string.Format(cmdFrame, "HOLD");
            }
            return null;
        }
        #endregion
    }
}
