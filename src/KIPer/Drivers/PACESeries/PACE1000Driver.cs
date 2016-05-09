using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using IEEE488;


namespace PACESeries
{
    public class PACE1000Driver:IDisposable
    {
        private readonly PACEParser _parser;
        private readonly ITransportIEEE488 _transport;
        private readonly int _address;

        public PACE1000Driver(int address, ITransportIEEE488 transport)
        {
            _transport = transport;
            _address = address;
        }

        public PACE1000Driver(ITransportIEEE488 transport):this(default (int), transport)
        {
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
            var idn = string.Empty;
            var cmd = _parser.GetCommandGetIdentificator();
            if (!_transport.Send(cmd))
                return null;
            var answer = _transport.Receive();
            if (!_parser.ParseGetIdentificator(answer, out idn))
                return string.Empty;
            return idn;
        }

        /// <summary>
        /// Получить текущее значение давления
        /// </summary>
        /// <returns></returns>
        public double GetPressure()
        {
            _transport.Send(_parser.GetCommandGetPressure());
            string m_strReturn = _transport.Receive();

            if (m_strReturn != null)
            {
                double? value;
                if (!_parser.ParseGetPressure(m_strReturn, out value))
                    return double.NaN;
                if (value != null) return value.Value;
            }
            return double.NaN;
        }

        /// <summary>
        /// Получить текущую дату на приборе
        /// </summary>
        /// <returns></returns>
        public DateTime? GetDate()
        {
            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;

            _transport.Send(_parser.GetCommandGetDate());
            string m_strDate = _transport.Receive();

            if (!_parser.ParseGetDate(m_strDate, out year, out month, out day))
                return null;

            _transport.Send(_parser.GetCommandGetTime());
            string m_strTime = _transport.Receive();

            if (!_parser.ParseGetTime(m_strTime, out hour, out min, out sec))
                return null;

            return new DateTime(year + 2000, month, day, hour, min, sec);
        }

        /// <summary>
        /// Получить диапазон измерения давления
        /// </summary>
        /// <returns></returns>
        public string GetPressureRange()
        {
            _transport.Send(_parser.GetCommandGetPressureRange());
            string m_strReturn = _transport.Receive();
            string result;
            if (!_parser.ParseGetPressureRange(m_strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Получить допустимые диапазоны измерения давления
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllPressureRanges()
        {
            _transport.Send(_parser.GetCommandGetAllRanges());
            string m_strReturn = _transport.Receive();
            IEnumerable<string> result;
            if (!_parser.ParseGetAllRanges(m_strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Установить диапазон давления
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool SetPressureRange(string range)
        {
            _transport.Send(_parser.GetCommandSetPressureRange(range));
            if (GetPressureRange() == range)
                return true;
            return false;
        }

        /// <summary>
        /// Получить единицы измерения давления
        /// </summary>
        /// <returns></returns>
        public PressureUnits? GetPressureUnit()
        {
            PressureUnits? result;
            _transport.Send(_parser.GetCommandGetPressureUnit());
            string m_strReturn = _transport.Receive();

            if (!_parser.ParseGetPressureUnit(m_strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Установить единицы измерения давления
        /// </summary>
        /// <returns></returns>
        public bool SetPressureUnit(PressureUnits unit)
        {
            _transport.Send(_parser.GetCommandSetPressureUnit(unit));
            var resUnit = GetPressureUnit();
            return resUnit != null && resUnit == unit;
        }

        /// <summary>
        /// Установить блокировку изменения режима управления пользователем
        /// </summary>
        public void SetLocalLockOutMode()
        {
            _transport.Send(_parser.GetCommandSetLocalLockOutMode());
        }

        /// <summary>
        /// Снять блокировку изменения режима управления пользователем
        /// </summary>
        public void SetOffLocalLockOutMode()
        {
            _transport.Send(_parser.GetCommandSetOffLocalLockOutMode());
        }

        /// <summary>
        /// Переключить в ручной режим управления
        /// </summary>
        public void SetLocal()
        {
            _transport.Send(_parser.GetCommandSetLocal());
        }

        /// <summary>
        /// Переключить в режим удаленного управления
        /// </summary>
        public void SetRemote()
        {
            _transport.Send(_parser.GetCommandSetRemote());
        }

        public void Dispose()
        {
            _transport.Close(_address);
        }
    }
}
