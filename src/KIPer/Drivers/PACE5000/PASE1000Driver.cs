using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using IEEE488;


namespace PACEVISADriver
{
    public class PASE1000Driver:IDisposable
    {
        private readonly ITransportIEEE488 _transport;
        private readonly int _address;

        public PASE1000Driver(int address, ITransportIEEE488 transport)
        {
            _transport = transport;
            _address = address;
        }

        public bool Open()
        {
            if (_transport == null)
                return false;
            return _transport.Open(_address);
        }

        /// <summary>
        /// Получить идентификатор устройства
        /// </summary>
        /// <returns></returns>
        public string GetIdentificator()
        {
            _transport.Send("*IDN?");
            string m_strReturn = _transport.Receive();

            return m_strReturn;
        }

        /// <summary>
        /// Получить текущее значение давления
        /// </summary>
        /// <returns></returns>
        public double GetPressure()
        {
            _transport.Send(":SENSe:PRESsure?");
            string m_strReturn = _transport.Receive();

            if (m_strReturn != null)
            {
                double value;
                var valStr = m_strReturn.Substring(11, m_strReturn.Length - 12);
                if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    return value;
            }
            return double.NaN;
        }

        /// <summary>
        /// Получить текущую дату на приборе
        /// </summary>
        /// <returns></returns>
        public DateTime? GetDate()
        {
            _transport.Send(":SYST:DATE?");
            string m_strDate = _transport.Receive();
            _transport.Send(":SYST:TIME?");
            string m_strTime = _transport.Receive();

            if (m_strDate != null)
            {
                DateTime? value = null;
                var valStrDate = m_strDate.Substring(11, m_strDate.Length - 12).Split(",".ToCharArray());
                var valStrTime = m_strTime.Substring(11, m_strTime.Length - 12).Split(",".ToCharArray());
                if (valStrDate.Length >= 3 && valStrTime.Length >= 3)
                {
                    int year = int.Parse(valStrDate[0]);
                    int month = int.Parse(valStrDate[1]);
                    int day = int.Parse(valStrDate[2]);
                    int hour = int.Parse(valStrTime[0]);
                    int min = int.Parse(valStrTime[1]);
                    int sec = int.Parse(valStrTime[2]);
                    return new DateTime(year+2000, month, day, hour, min, sec);
                }
                    return value;
            }
            return null;
        }

        /// <summary>
        /// Получить единицы измерения давления
        /// </summary>
        /// <returns></returns>
        public string GetPressureUnit()
        {
            _transport.Send(":UNIT:PRES?");
            string m_strReturn = _transport.Receive();

            if (m_strReturn != null)
            {
                return m_strReturn.Substring(11, m_strReturn.Length - 12);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Установить единицы измерения давления
        /// </summary>
        /// <returns></returns>
        public bool SetPressureUnit(string unit)
        {
            _transport.Send(string.Format(":UNIT:PRES {0}", unit));
            var resUnit = GetPressureUnit();
            return resUnit != null && resUnit == unit;
        }

        /// <summary>
        /// Получить диапазон измерения текущего канала
        /// </summary>
        /// <returns></returns>
        public string GetUnitSpeed()
        {
            _transport.Send("UNIT:SPEed?");
            string m_strReturn = _transport.Receive();

            if (m_strReturn != null)
            {
                return m_strReturn.Substring(10, m_strReturn.Length - 11);
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            _transport.Close(_address);
        }
    }
}
