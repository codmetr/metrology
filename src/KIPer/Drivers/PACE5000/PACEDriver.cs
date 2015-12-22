using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PACESeries
{
    public class PACEDriver
    {
        protected virtual void Send(string command)
        {
            
        }

        protected virtual string Receive()
        {
            return null;
        }

        public string SendCommand(string command)
        {
            Send(command);
            return Receive();
        }
        

        int GetNumberCalibratinPoints(int channel)
        {
            var commandBase = string.Format(":CAL{0}:PRES:POIN", channel);
            var command = string.Format("{0}?", commandBase);
            var answer = SendCommand(command);
            if (!answer.StartsWith(commandBase))
                throw new AnswerException(commandBase, answer);
            var stringValue = answer.Remove(0, commandBase.Length);
            int result;
            if(!int.TryParse(stringValue, out result))
                throw new AnswerException(commandBase, answer);
            return result;
        }

        int GetNumberCalibratinPoints(int channel)
        {
            var commandBase = string.Format(":CAL{0}:PRES:POIN", channel);
            var command = string.Format("{0}?", commandBase);
            var answer = SendCommand(command);
            if (!answer.StartsWith(commandBase))
                throw new AnswerException(commandBase, answer);
            var stringValue = answer.Remove(0, commandBase.Length);
            int result;
            if(!int.TryParse(stringValue, out result))
                throw new AnswerException(commandBase, answer);
            return result;
        }
    }
}
