using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DPI620Genii
{
    public class DPI620DriverUsb : IDPI620Driver
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

        private FileStream _file;
        private StreamWriter _writer;
        private StreamReader _reader;
        public void Open()
        {
            _file = File.Create("\\\\.\\wceusbsh001", 100, FileOptions.RandomAccess);
            _writer = new StreamWriter(_file, Encoding.UTF8);
            _reader = new StreamReader(_file, Encoding.UTF8);
            _writer.Write("#km=r\r\n");
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

        public void SetUnits(int slotId, string unitCode)
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
                _writer.Write("#km=l\r\n");
                _file.Close();
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
            }
        }
    }
}
