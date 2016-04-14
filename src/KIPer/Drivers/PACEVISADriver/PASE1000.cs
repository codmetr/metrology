using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VisaDriver;


namespace PACEVISADriver
{
    public class PASE1000:IDisposable
    {
        private readonly Visa _transport;

        public PASE1000(Visa transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Получить идентификатор устройства
        /// </summary>
        /// <returns></returns>
        public string GetIdentificator()
        {
            _transport.WriteString("*IDN?");
            string m_strReturn = _transport.ReadString();

            if (m_strReturn != null)
            {
                return m_strReturn;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получить текущее значение давления
        /// </summary>
        /// <returns></returns>
        public double GetPressure()
        {
            _transport.WriteString(":SENSe:PRESsure?");
            string m_strReturn = _transport.ReadString();

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
            _transport.WriteString(":SYST:DATE?");
            string m_strDate = _transport.ReadString();
            _transport.WriteString(":SYST:TIME?");
            string m_strTime = _transport.ReadString();

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
            _transport.WriteString(":UNIT:PRES?");
            string m_strReturn = _transport.ReadString();

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
            _transport.WriteString(string.Format(":UNIT:PRES {0}", unit));
            var resUnit = GetPressureUnit();
            return resUnit != null && resUnit == unit;
        }

        /// <summary>
        /// Получить диапазон измерения текущего канала
        /// </summary>
        /// <returns></returns>
        public string GetUnitSpeed()
        {
            _transport.WriteString("UNIT:SPEed?");
            string m_strReturn = _transport.ReadString();

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
            _transport.Dispose();
        }
    }
}
