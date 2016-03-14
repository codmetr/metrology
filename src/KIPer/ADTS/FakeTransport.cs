using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class FakeTransport:IEEE488.ITransportIEEE488
    {
        private string request;
        private string answer;

        private string _unit = "PSI";
        private IDictionary<string, string> _rate = new Dictionary<string, string>();
        private IDictionary<string, string> _aim = new Dictionary<string, string>();


        public bool Open(int address)
        {
            return true;
        }

        public bool Close(int address)
        {
            return true;
        }

        public bool Send(int address, string data)
        {
            request = data;
            answer = GetFakeAnswer(request);
            return true;
        }

        public string Receive(int address)
        {
            return answer;
        }

        private string GetFakeAnswer(string request)
        {
            switch (request)
            {
                case ADTSParser.KeyGetDate: return string.Format("{0:0000},{1:00},{1:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                case ADTSParser.KeySetState: return "";
                case ADTSParser.KeyGetState: return "ON";
                case ADTSParser.KeyStartMainCalibration: return "";
                case ADTSParser.KeySetValueActualPressure: return "";
                case ADTSParser.KeyGetCalibrationResult: return "0.5,0";
                case ADTSParser.KeySetMainCalibrationAccept: return "";
                case ADTSParser.KeySetUnitPressure: _unit = request.Substring("UNIT:PRES ".Length); return "";
                case ADTSParser.KeyGetUnitPressure: return _unit;
                case ADTSParser.KeySetRate: 
                    var rate = request.Substring("SOUR:RATE ".Length).Split(",".ToCharArray());
                    if (_rate.ContainsKey(rate[0]))
                        _rate[rate[0]] = rate[1];
                    else
                        _rate.Add(rate[0], rate[1]);
                    return "";
                case ADTSParser.KeyGetRate:
                    var rateParam = request.Substring("SOUR:RATE? ".Length);
                    return _rate.ContainsKey(rateParam) ? _rate[rateParam] : "100";
                case ADTSParser.KeySetAim:
                    var aim = request.Substring("SOUR:PRES ".Length).Split(",".ToCharArray());
                    if (_aim.ContainsKey(aim[0]))
                        _aim[aim[0]] = aim[1];
                    else
                        _aim.Add(aim[0], aim[1]);
                    return "";
                case ADTSParser.KeyGetAim:
                    var aimParam = request.Substring("SOUR:PRES? ".Length);
                    return _aim.ContainsKey(aimParam) ? _aim[aimParam] : "100";
                case ADTSParser.KeyGetStatusOfADTS: return "10";
                case ADTSParser.KeyGetPressure:
                    var presParam = request.Substring("MEAS:PRES? ".Length);
                    return _aim.ContainsKey(presParam) ? _aim[presParam] : "100";
            }
            return null;
        }
    }
}
