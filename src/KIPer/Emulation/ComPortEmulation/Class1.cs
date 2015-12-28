using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComPortEmulation
{
    public class ComPort
    {
        private readonly Func<byte[], byte[]> _getAnswer;

        public ComPort(Func<byte[], byte[]> getAnswer)
        {
            _getAnswer = getAnswer;
        }

        public byte[] Send(byte[] message)
        {
            return _getAnswer(message);
        }
    }
}
