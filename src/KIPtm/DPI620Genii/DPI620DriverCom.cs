﻿using System;
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
        private StreamWriter _writer;
        private StreamReader _reader;
        private Action<string> _toLog = s => { };

        public DPI620DriverCom()
        {
            
        }

        //public void Open(string portName)
        //{
        //    _serial = new SerialPort(portName);
        //    _serial.BaudRate = 19200;
        //    _writer = new StreamWriter(_serial.BaseStream, Encoding.Unicode);
        //    _reader = new StreamReader(_serial.BaseStream, Encoding.Unicode);

        //    Write("*km=r\r\n");
        //}

        //public bool TryToIdentifyCOM(String port)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        Open(port);

        //        Thread.Sleep(100);

        //        Write("*ri?\r\n");

        //        Thread.Sleep(400);
        //        string readedLine;
        //        while ((readedLine = Read()).Length > 0) {
        //            sb.Append(readedLine);
        //        }
        //        Write("*km=l\r\n");
        //        if (sb.ToString().ToLower().Contains("dpi620"))
        //        {
        //            return true;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return false;
        //}

        public DPI620DriverCom Setlog(Action<string> toLog)
        {
            _toLog = toLog;
            return this;
        }

        private void Log(string msg)
        {
            _toLog(msg);
        }

        private void Write(string data)
        {
            _serial.Write(data);
            //_writer.Write(data);
            Log($">>{data} | {DataToHex(data)}");
        }

        private string Read()
        {
            var line = _serial.ReadLine();
            Log($"<<{line} | {DataToHex(line)}");
            return line;
        }

        public void SetPort(string portName)
        {
            _serial = new SerialPort(portName, 19200, Parity.None, 8, StopBits.One);
            _serial.RtsEnable = false;
            _serial.DtrEnable = true;
            _serial.ReadTimeout = 1000;
            _serial.WriteTimeout = 1000;
            Log("Set port: " + portName);
        }

        public void Open()
        {
            try
            {
                _serial.Open();
                Log("Serial \"" + _serial.PortName + "\" open");
                _writer = new StreamWriter(_serial.BaseStream, Encoding.UTF8);
                _reader = new StreamReader(_serial.BaseStream, Encoding.UTF8);
                Write("*km=r\r\n");
                Write("*su3=1\r\n");
                Write("*km=r\r\n");
                //Write("*ri?\r\n");
                var dpiType = Read();
                if (dpiType.Contains("DPI"))
                {
                    var tName = dpiType.Split('=')[1];
                    Log($"Device {tName} connected");
                }
                ReadConf();
                //Read();
                //Read();
                //Read();
                //Read();
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
        private void ReadConf()
        {
            // канал 0
            Write("*pr0?\r\n");
            try
            {
                var ch0Descr = Read();//TODO: parce
            }
            catch (Exception e)
            {
                Log($"On read config channel 0 error: {e.ToString()}");
                //_startIndex = 1;
            }

            // канал 1
            Write("*pr1?\r\n");
            try
            {
                var ch1Descr = Read();//TODO: parce
            }
            catch (Exception e)
            {
                Log($"On read config channel 1 error: {e.ToString()}");
            }

            // канал 2
            Write("*pr2?\r\n");
            try
            {
                var ch2Descr = Read();//TODO: parce
            }
            catch (Exception e)
            {
                Log($"On read config channel 2 error: {e.ToString()}");
            }
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

        //public void SetUnits(int slotId, String unitCode)
        //{
        //    Log("DPI620 UNIT CODE " + unitCode);

        //    if (!_dicCmdSetUnit.ContainsKey(unitCode))
        //        return;
        //    try
        //    {
        //        Write(_dicCmdSetUnit[unitCode]);
        //    }
        //    catch (IOException ex)
        //    {
        //        Log(ex.ToString());
        //    }
        //}

        /// <summary>
        /// Получить значение
        /// </summary>
        /// <param name="slotId">Номер слота (c 1)</param>
        /// <returns></returns>
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
                var valstr = sb.Split("=".ToCharArray())[1].Trim();
                Log("Value = \"" + valstr + "\"");
                double val = double.Parse(valstr, NumberStyles.Any, CultureInfo.InvariantCulture);

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
            return 0.0D;
        }

        public void Close()
        {
            try
            {
                Write("#km=l\r\n");
                Log("Serial \"" + _serial.PortName + "\" close");
                _serial.Close();
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
            }
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
    }
}
