using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace DPI620Genii
{
    public class DPI620DriverCom : IDPI620Driver
    {
        readonly Dictionary<string, string> _dicCmdSetUnit = new Dictionary<string, string>()
        {
            {"UPBA", "#su3=1\n\r"},
            {"UPMA", "#su3=0\n\r"},
            {"UPAM", "#su3=5\n\r" },
            {"UPPA", "#su3=2\n\r" },
            {"UPKA", "#su3=4\n\r" },
            {"UPHP", "#su3=3\n\r" },
            {"UPKG", "#su3=10\n\r" },
            {"UPMM",  "#su3=12\n\r"},
            {"UPCM", "#su3=13\n\r" },
            {"UPMH", "#su3=14\n\r" },
            {"UPHG",  "#su3=6\n\r"},
            {"UPIH", "#su3=9\n\r" },
            {"UPTR", "#su3=15\n\r" },
            {"UPAT", "#su3=16\n\r" },
            { "UPPS", "#su3=17\n\r"},
        };

        private SerialPort _serial;
        private Action<string> _toLog = s => { };
        private Action<string> _toLogTrace = s => { };
        private bool _isOpen = false;

        public DPI620DriverCom()
        {
            
        }

        /// <inheritdoc />
        public void Open()
        {
            if(_isOpen)
                return;
            try
            {
                Clear();
                Log("Serial \"" + _serial.PortName + "\" open");
                Write("*km=r\r\n");
                Write("*su3=1\r\n");
                Write("*km=r\r\n");
                var dpiType = Read();
                if (dpiType.Contains("DPI"))
                {
                    var tName = dpiType.Split('=')[1];
                    Log($"Device {tName} connected");
                }
                ReadConf();
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
                throw;
            }
            catch (Exception e)
            {
                Log(e.ToString());
                throw;
            }
            _isOpen = true;
        }

        /// <inheritdoc />
        public IEnumerable<int> TestSlots()
        { 
        
            IEnumerable<int> res = new int[0];
            try
            {
                Log("Serial \"" + _serial.PortName + "\" open");
                Write("*km=r\r\n");
                Write("*su3=1\r\n");
                Write("*km=r\r\n");
                var dpiType = Read();
                if (dpiType.Contains("DPI"))
                {
                    var tName = dpiType.Split('=')[1];
                    Log($"Device {tName} connected");
                }
                res = ReadConf();
                Close();
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
                throw;
            }
            catch (Exception e)
            {
                Log(e.ToString());
                throw;
            }
            return res;
        }

        /// <inheritdoc />
        public double GetValue(int slotId/*, string unitCode*/)
        {
            try
            {
                //if (("UCMA" == unitCode) || ("UVVO" == unitCode) || ("UVMV" == unitCode))
                var slotindex = slotId;
                Write($"*ir{slotindex}?\r\n");
                Read();
                var sb = Read();
                //Log("RAW620: " + unitCode + " " + sb);
                var strParts = sb.Split("=".ToCharArray());
                if (!sb.Contains("=")|| strParts.Length<2)
                {
                    Log(string.Format("[ERROR] Receiver error answer: \'{0}\' (not fount simbol \'=\')", sb));
                    return Double.NaN;
                }

                var valstr = sb.Split("=".ToCharArray())[1].Trim();
                Log($"Value[{slotId}] = \"{valstr}\"");

                double val;
                if (!double.TryParse(valstr, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                {
                    Log("[ERROR] Can not parse \"" + valstr + "\" to string");
                    return double.NaN;
                }
                return val;
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
                throw;
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
            return Double.NaN;
        }

        /// <inheritdoc />
        public void Close()
        {
            if(!_isOpen)
                return;
            try
            {
                Write("#km=l\r\n");
                Log("Serial \"" + _serial.PortName + "\" close");
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
            }
            _isOpen = false;
        }

        public DPI620DriverCom Setlog(Action<string> toLog, Action<string> toLogTrace = null)
        {
            _toLog = toLog;
            _toLogTrace = toLogTrace ?? _toLog;
            return this;
        }

        private void Log(string msg)
        {
            _toLog(msg);
        }

        private void LogTrace(string msg)
        {
            _toLogTrace(msg);
        }

        private void Clear()
        {
            _serial.DiscardInBuffer();
            _serial.DiscardOutBuffer();
            LogTrace($"clear all buffer");
        }

        private void Write(string data)
        {
            _serial.Write(data);
            var datastr = data.Replace("\r", "\\r").Replace("\n", "\\n");
            LogTrace($">>{datastr} | {DataToHex(data)}");
        }

        private string Read()
        {
            var line = _serial.ReadLine();
            var datastr = line.Replace("\r", "\\r").Replace("\n", "\\n");
            LogTrace($"<<{datastr} | {DataToHex(line)}");
            return line;
        }

        public void SetPort(SerialPort port)
        {
            _serial = port;
            _serial.RtsEnable = false;
            _serial.DtrEnable = true;
            _serial.ReadTimeout = 1000;
            _serial.WriteTimeout = 1000;
            Log("Set port: " + _serial.PortName);
        }

        /// <summary>
        /// Прочитать конфигацию
        /// </summary>
        /// <remarks>
        /// *pc?                                    //конфигурация по текущему каналу?
        /// !PC=0.000000,20.000000,10,10.000000,    //
        /// *re?                                    //последняя ошибка?
        /// !RE=00000000                            //нет ошибок        
        /// *pc0?                                   //конфигурация по каналу 0?
        /// !PC=0.000000,20.000000,10,10.000000,    //
        /// *re?                                    //последняя ошибка?                                        
        /// !RE=00000000                            //                                        
        /// *pc1?                                   //конфигурация по каналу 1?    
        /// !PC=0.000000,0.000000,0,0.000000,       //
        /// *re?                                    //последняя ошибка?                       
        /// !RE=00000000                            //                                        
        /// *pc2?                                   //конфигурация по каналу 2?       
        ///     * re?                                    //последняя ошибка?                       
        /// !RE=00000002                            //что-то вроде "недопустимая команда"
        /// *re?                                    //последняя ошибка?
        /// !RE=00000000                            //                       
        /// *pc2?                                   //                                        
        ///     * re?                                    //последняя ошибка?                       
        /// !RE=00000002                            //что-то вроде "недопустимая команда"
        /// 
        /// этот лог обозначает - настроены каналы 0,1, канал 2 - не задействован(видимо источник)
        /// </remarks>
        private IEnumerable<int> ReadConf()
        {
            var res = new List<int>();
            // канал 0
            var getCh0Descr = "*pr0?\r\n";
            Write(getCh0Descr);
            try
            {
                var ch0repeat = Read();//TODO: parce
                var ch0Descr = Read();
                if(ch0repeat == getCh0Descr && ch0Descr.StartsWith("!PR="))
                    res.Add(0);
            }
            catch (Exception e)
            {
                Log($"On read config channel 0 error: {e.ToString()}");
                //_startIndex = 1;
            }

            // канал 1
            var getCh1Descr = "*pr1?\r\n";
            Write(getCh1Descr);
            try
            {
                var ch1repeat = Read();//TODO: parce
                var ch1Descr = Read();
                if (ch1repeat == getCh1Descr && ch1Descr.StartsWith("!PR="))
                    res.Add(1);
            }
            catch (Exception e)
            {
                Log($"On read config channel 1 error: {e.ToString()}");
            }

            // канал 2
            var getCh2Descr = "*pr1?\r\n";
            Write(getCh2Descr);
            try
            {
                var ch2repeat = Read();//TODO: parce
                var ch2Descr = Read();
                if (ch2repeat == getCh2Descr && ch2Descr.StartsWith("!PR="))
                    res.Add(2);
            }
            catch (Exception e)
            {
                Log($"On read config channel 2 error: {e.ToString()}");
            }
            return res;
        }

        /// <summary>
        /// Проверка связи
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            try
            {
                Write("*ri?\r\n");
                var dpiType = Read();
                if (dpiType.Contains("DPI"))
                {
                    var tName = dpiType.Split('=')[1];
                    Log($"Ping: {tName} Ok");
                    return true;
                }
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
            return false;
        }

        private string DataToHex(string str)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                sb.Append(((byte)c).ToString("X2"));
                sb.Append(" ");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            if (_serial == null)
                return $"DPI620";
            return $"DPI620 (port {_serial.PortName}, rate {_serial.BaudRate}, data {_serial.DataBits}, parity {_serial.Parity})";
        }
    }
}
