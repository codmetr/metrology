using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Ivi.Visa.Interop;


namespace PACEVISADriver
{
    public class PASE1000:IDisposable
    {
        private Ivi.Visa.Interop.ResourceManager rm;
        private Ivi.Visa.Interop.FormattedIO488 ioArbFG;
        private Ivi.Visa.Interop.IMessage msg;

        public PASE1000(string address)
        {
            try
            {
                rm = new ResourceManager();
                ioArbFG = new FormattedIO488Class();
                this.msg = (rm.Open(address, Ivi.Visa.Interop.AccessMode.NO_LOCK, 2000, "")) as IMessage;
                this.ioArbFG.IO = msg;
            }
            catch (SystemException ex)
            {
                this.ioArbFG.IO = null;
                throw;
            }
        }

        /// <summary>
        /// Получить идентификатор устройства
        /// </summary>
        /// <returns></returns>
        public string GetIdentificator()
        {
            ioArbFG.WriteString("*IDN?", true);
            string m_strReturn = ioArbFG.ReadString();

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
            ioArbFG.WriteString(":SENSe:PRESsure?", true);
            string m_strReturn = ioArbFG.ReadString();

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
            ioArbFG.WriteString(":SYST:DATE?", true);
            string m_strDate= ioArbFG.ReadString();
            ioArbFG.WriteString(":SYST:TIME?", true);
            string m_strTime= ioArbFG.ReadString();

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
            ioArbFG.WriteString(":UNIT:PRES?", true);
            string m_strReturn = ioArbFG.ReadString();

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
            ioArbFG.WriteString(string.Format(":UNIT:PRES {0}", unit), true);
            var resUnit = GetPressureUnit();
            return resUnit != null && resUnit == unit;
        }

        /// <summary>
        /// Получить диапазон измерения текущего канала
        /// </summary>
        /// <returns></returns>
        public string GetUnitSpeed()
        {
            ioArbFG.WriteString("UNIT:SPEed?", true);
            string m_strReturn = ioArbFG.ReadString();

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
            ioArbFG.IO.Close();
        }
    }
}
