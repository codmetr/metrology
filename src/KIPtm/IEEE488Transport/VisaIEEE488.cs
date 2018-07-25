using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using VisaDriver;

namespace IEEE488
{
    public class VisaIEEE488 : ITransportIEEE488
    {
        private VisaDriver.Visa _visa;
        
        /// <summary>
        ///  Транспортный уровень VISA
        /// </summary>
        public VisaIEEE488()
        {
            _visa = null;
        }

        /// <summary>
        /// Открыть подключение
        /// </summary>
        /// <param name="address">Адрес прибора</param>
        /// <returns>true - Удалось подключиться</returns>
        public bool Open(string address)
        {
            if (_visa == null)
            {
                _visa = new Visa(address);
                return true;
            }
            return _visa.SetAddress(address);
        }

        /// <summary>
        /// Открыть подключение
        /// </summary>
        /// <param name="address">Адрес прибора</param>
        /// <returns>true - Удалось подключиться</returns>
        public bool Open(int address)
        {
            return Open(string.Format("GPIB0::{0}", address));
        }

        /// <summary>
        /// Закрыть подключение
        /// </summary>
        /// <returns>true - Удалось отключиться без ошибок</returns>
        public bool Close()
        {
            if (_visa != null)
                _visa.Dispose();
            _visa = null;
            return true;
        }

        /// <summary>
        /// Закрыть подключение
        /// </summary>
        /// <param name="address"></param>
        /// <returns>true - Удалось отключиться без ошибок</returns>
        public bool Close(int address)
        {
            return Close();
        }

        /// <summary>
        /// Посылка команды
        /// </summary>
        /// <param name="data">Команда</param>
        /// <returns>Удалось послать команду</returns>
        public bool Send(string data)
        {
            if (_visa == null)
                throw new Exception("Call Send before \"Open\"");
            _visa.WriteString(data);
            return true;
        }

        /// <summary>
        /// Чтение ответа
        /// </summary>
        /// <returns>Ответ</returns>
        public string Receive()
        {
            if (_visa == null)
                throw new Exception("Call Receive before \"Open\"");
            return _visa.ReadString();
        }
    }
}
