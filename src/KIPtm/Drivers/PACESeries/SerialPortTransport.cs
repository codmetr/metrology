using System;
using System.IO.Ports;
using System.Linq;
using IEEE488;

namespace PACESeries
{
    public class SerialPortTransport : ITransport
    {
        private readonly SerialPort _port;
        private readonly Action<string> _log;

        public SerialPortTransport(SerialPort port, Action<string> log = null)
        {
            _port = port;
            _log = log;
        }

        public bool Send(string data)
        {
            try
            {
                //var buff = $"{data}\n".ToCharArray();
                //_port.Write(buff, 0, buff.Length);
                //var bufStr = string.Join(" ", buff.Select(el => ((int)el).ToString("X")));
                //Log($"[{DateTime.Now}]>>{data}|{bufStr}");
                _port.WriteLine(data);
                Log($"[{DateTime.Now}]>>{data}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log(e.ToString());
                throw;
            }
            return true;
        }

        public string Receive()
        {
            var line = _port.ReadLine();
            Log($"[{DateTime.Now}]<<{line}");
            return line;
        }

        private void Log(string msg)
        {
            _log?.Invoke(msg);
        }
    }
}