using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PACESeries
{
    public class PACEDriver
    {
        private readonly int _address;
        private readonly ITransport _transport;

        public PACEDriver(int address, ITransport transport)
        {
            _address = address;
            _transport = transport;
        }

        public string SendCommand(string command)
        {
            _transport.Send(command, _address);
            return _transport.Receive(_address);
        }

        public int GetNumberCalibratinPoints(int channel)
        {
            return GetNumberCalibratinPoints(channel, SendCommand);
        }

        public int GetNumberCalibratinPoints(int channel, Func<string, string> sendFunc)
        {
            var commandBase = string.Format(":CAL{0}:PRES:POIN", channel);
            var command = string.Format("{0}?", commandBase);
            var answer = sendFunc(command);
            if (!answer.StartsWith(commandBase))
                throw new AnswerException(commandBase, answer);
            var stringValue = answer.Remove(0, commandBase.Length);
            int result;
            if(!int.TryParse(stringValue, out result))
                throw new AnswerException(commandBase, answer);
            return result;
        }

        public int GetAltetude()
        {
            return GetAltetude(SendCommand);
        }

        public int GetAltetude(Func<string, string> sendFunc)
        {
            const string command = "SENS:ALT?";
            var answer = sendFunc(command);
            const string answerBase = "SENS:ALT ";
            if (!answer.StartsWith(answerBase))
                throw new AnswerException(answerBase, answer);
            var stringValue = answer.Remove(0, answerBase.Length);
            int result;
            if(!int.TryParse(stringValue, out result))
                throw new AnswerException(answerBase, answer);
            return result;
        }
    }
}
