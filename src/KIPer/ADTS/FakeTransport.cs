using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADTS
{
    public class FakeTransport : IEEE488.ITransportIEEE488
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

        public bool Send(string data)
        {
            request = data;
            answer = GetFakeAnswer(request);
            return true;
        }

        public string Receive()
        {
            return answer;
        }

        private string GetFakeAnswer(string request)
        {
            var reqArr = request.Split(" ".ToCharArray());
            switch (reqArr[0])
            {
                case ADTSParser.KeyGetDate: return string.Format("{0:0000},{1:00},{1:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                case ADTSParser.KeyGetState: return "ON";
                case ADTSParser.KeyGetCalibrationResult: return "0.5,0";
                case ADTSParser.KeyGetUnitPressure: return _unit;
                case ADTSParser.KeyGetStatusOfADTS: return "10";
                default:
                    if (reqArr[0] == ADTSParser.KeySetState.Split(" ".ToCharArray())[0])
                        return "";
                    else if (reqArr[0] == ADTSParser.KeyStartMainCalibration.Split(" ".ToCharArray())[0])
                        return "";
                    else if (reqArr[0] == ADTSParser.KeySetValueActualPressure.Split(" ".ToCharArray())[0])
                        return "";
                    else if (reqArr[0] == ADTSParser.KeySetMainCalibrationAccept.Split(" ".ToCharArray())[0])
                        return "";
                    else if (reqArr[0] == ADTSParser.KeySetUnitPressure.Split(" ".ToCharArray())[0])
                    {
                        _unit = request.Substring("UNIT:PRES ".Length);
                        return "";
                    }
                    else if (reqArr[0] == ADTSParser.KeySetRate.Split(" ".ToCharArray())[0])
                    {
                        var rate = request.Substring("SOUR:RATE ".Length).Split(",".ToCharArray());
                        if (_rate.ContainsKey(rate[0]))
                            _rate[rate[0]] = rate[1];
                        else
                            _rate.Add(rate[0], rate[1]);
                        return "";
                    }
                    else if (reqArr[0] == ADTSParser.KeyGetRate.Split(" ".ToCharArray())[0])
                    {
                        var rateParam = request.Substring("SOUR:RATE? ".Length);
                        return _rate.ContainsKey(rateParam) ? _rate[rateParam] : "100";
                    }
                    else if (reqArr[0] == ADTSParser.KeySetAim.Split(" ".ToCharArray())[0])
                    {
                        var aim = request.Substring("SOUR:PRES ".Length).Split(",".ToCharArray());
                        if (_aim.ContainsKey(aim[0]))
                            _aim[aim[0]] = aim[1];
                        else
                            _aim.Add(aim[0], aim[1]);
                        return "";
                    }
                    else if (reqArr[0] == ADTSParser.KeyGetAim.Split(" ".ToCharArray())[0])
                    {
                        var aimParam = request.Substring("SOUR:PRES? ".Length);
                        return _aim.ContainsKey(aimParam) ? _aim[aimParam] : "100";
                    }
                    else if (reqArr[0] == ADTSParser.KeyGetPressure.Split(" ".ToCharArray())[0])
                    {
                        var presParam = request.Substring("MEAS:PRES? ".Length);
                        return _aim.ContainsKey(presParam) ? _aim[presParam] : "100";
                    }
                    break;
            }
            return null;
        }
    }
}
