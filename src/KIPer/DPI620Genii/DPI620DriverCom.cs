﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace DPI620Genii
{
    class DPI620DriverCom
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
        public void Open(string portName)
        {
            SerialPort serial = new SerialPort(portName);
            serial.BaudRate = 19200;
            _writer = new StreamWriter(serial.BaseStream, Encoding.UTF8);
            _reader = new StreamReader(serial.BaseStream, Encoding.UTF8);

            _writer.Write("#km=r\r\n");
        }

        public bool TryToIdentifyCOM(String port)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                Open(port);

                Thread.Sleep(100);

                Write("*ri?\r\n");
                
                Thread.Sleep(400);
                string readedLine;
                while ((readedLine = _reader.ReadLine()).Length > 0) {
                    sb.Append(readedLine);
                }
                Write("*km=l\r\n");
                if (sb.ToString().ToLower().Contains("dpi620"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private void Log(string msg)
        {

        }

        private void Write(string data)
        {
            _writer.Write(data);
        }

        private string Read()
        {
            return _reader.ReadLine();
        }

        public void SetUnits(int slotId, String unitCode)
        {
            Log("DPI620 UNIT CODE " + unitCode);

            if (!_dicCmdSetUnit.ContainsKey(unitCode))
                return;
            try
            {
                Write(_dicCmdSetUnit[unitCode]);
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
            }
        }

        public double GetValue(int slotId, string unitCode)
        {
            try
            {
                if (("UCMA" == unitCode) || ("UVVO" == unitCode) || ("UVMV" == unitCode))
                {
                    Write("#ir1?\r\n");
                }
                else
                {
                    Write("#ir2?\r\n");
                }

                var sb = Read();
                Log("RAW620: " + unitCode + " " + sb);
                double val = double.Parse(sb.Split("=".ToCharArray())[1]);

                return val;
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
                throw;
            }
            catch (Exception e) { }
            return 0.0D;
        }

        public void Close()
        {
            try
            {
                _writer.Write("#km=l\r\n");
                _serial.Close();
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
            }
        }
    }
}