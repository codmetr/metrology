﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using IEEE488;

namespace PACESeries
{
    /// <summary>
    /// Драйвер PACE1000
    /// </summary>
    public class PACE1000Driver:IDisposable
    {
        private readonly PACEParserV2 _parser;
        //private readonly PACEParser _parser;
        private readonly ITransport _transport;
        
        /// <summary>
        /// Драйвер PACE1000
        /// </summary>
        /// <param name="transport">Канал обмена</param>
        public PACE1000Driver(ITransport transport)
        {
            _transport = transport;
            _parser = new PACEParserV2();
            //_parser = new PACEParser();
        }

        /// <summary>
        /// Драйвер PACE1000
        /// </summary>
        /// <param name="port">Канал обмена</param>
        /// <param name="log">лог</param>
        public PACE1000Driver(SerialPort port, Action<string> log = null)
        {
            _transport = new SerialPortTransport(port, log);
            _parser = new PACEParserV2();
            //_parser = new PACEParser();
        }

        /// <summary>
        /// Получить идентификатор устройства
        /// </summary>
        /// <returns>Идентификатор</returns>
        [DefModel(Model.PACE1000|Model.PACE5000|Model.PACE6000)]
        public string GetIdentificator()
        {
            var idn = string.Empty;
            var cmd = _parser.GetCommandGetIdentificator();
            if (!Send(cmd))
                return null;
            var answer = Receive();
            if (!_parser.ParseGetIdentificator(answer, out idn))
                return string.Empty;
            return idn;
        }

        /// <summary>
        /// Получить текущее значение давления
        /// </summary>
        /// <returns>Значение давления</returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public double GetPressure()
        {
            Send(_parser.GetCommandGetPressure());
            string strReturn = Receive();

            if (strReturn != null)
            {
                double? value;
                if (!_parser.ParseGetPressure(strReturn, out value))
                    return double.NaN;
                if (value != null) return value.Value;
            }
            return double.NaN;
        }

        /// <summary>
        /// Получить текущую дату на приборе
        /// </summary>
        /// <returns>Дата на приборе</returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public DateTime? GetDate()
        {
            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;

            Send(_parser.GetCommandGetDate());
            string strDate = Receive();

            if (!_parser.ParseGetDate(strDate, out year, out month, out day))
                return null;

            Send(_parser.GetCommandGetTime());
            string strTime = Receive();

            if (!_parser.ParseGetTime(strTime, out hour, out min, out sec))
                return null;

            return new DateTime(year + 2000, month, day, hour, min, sec);
        }

        /// <summary>
        /// Получить диапазон измерения давления
        /// </summary>
        /// <returns>Диапазон давления</returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public string GetPressureRange()
        {
            Send(_parser.GetCommandGetPressureRange());
            string strReturn = Receive();
            string result;
            if (!_parser.ParseGetPressureRange(strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Получить допустимые диапазоны измерения давления
        /// </summary>
        /// <returns>Список поддерживаемых диапазонов</returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public IEnumerable<string> GetAllPressureRanges()
        {
            Send(_parser.GetCommandGetAllRanges());
            string m_strReturn = Receive();
            IEnumerable<string> result;
            if (!_parser.ParseGetAllRanges(m_strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Установить диапазон давления
        /// </summary>
        /// <param name="range">Диапазон давления из списка <see cref="GetAllPressureRanges"/></param>
        /// <returns></returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public bool SetPressureRange(string range)
        {
            Send(_parser.GetCommandSetPressureRange(range));
            if (GetPressureRange() == range)
                return true;
            return false;
        }

        /// <summary>
        /// Получить единицы измерения давления
        /// </summary>
        /// <returns>Единицы давления</returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public PressureUnits? GetPressureUnit()
        {
            PressureUnits? result;
            Send(_parser.GetCommandGetPressureUnit());
            string m_strReturn = Receive();

            if (!_parser.ParseGetPressureUnit(m_strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Установить единицы измерения давления
        /// </summary>
        /// <param name="unit">Единицы давления</param>
        /// <returns>Удалось установить единицы давления</returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public bool SetPressureUnit(PressureUnits unit)
        {
            Send(_parser.GetCommandSetPressureUnit(unit));
            var resUnit = GetPressureUnit();
            return resUnit != null && resUnit == unit;
        }

        /// <summary>
        /// Установить целевой точки давления
        /// </summary>
        /// <param name="value">Целевая точка давления</param>
        /// <returns>Удалось установить целевую точку давления</returns>
        [DefModel(Model.PACE5000|Model.PACE6000)]
        public bool SetPressure(double value)
        {
            Send(_parser.GetCommandSetAimPressure(value));
            return true;
            //TODO реализовать проверку установки цели
            //var resUnit = GetPressureUnit();
            //return resUnit != null && resUnit == unit;
        }

        /// <summary>
        /// Установить целевой точки давления
        /// </summary>
        /// <returns>Удалось установить целевую точку давления</returns>
        [DefModel(Model.PACE5000|Model.PACE6000)]
        public bool GetAimPressure(out double aim)
        {
            Send(_parser.GetCommandGetAimPressure());
            return _parser.ParseGetAimPressure(Receive(), out aim);
        }

        /// <summary>
        /// Установка режима контроля по каналу
        /// </summary>
        /// <param name="state">состояние</param>
        /// <returns></returns>
        [DefModel(Model.PACE5000 | Model.PACE6000)]
        public bool SetOutputState(bool state)
        {
            Send(_parser.GetCommandSetOutputState(state));
            return true;
            //TODO реализовать проверку установки состояния канала
        }

        /// <summary>
        /// Прочитать режим
        /// </summary>
        /// <param name="state">состояние</param>
        /// <returns></returns>
        [DefModel(Model.PACE5000 | Model.PACE6000)]
        public bool GetOutputState(out bool state)
        {
            Send(_parser.GetCommandGetOutputState());
            return _parser.ParseGetOutputState(Receive(), out state);
            //TODO реализовать проверку установки состояния канала
        }

        /// <summary>
        /// Установка режима контроля по каналу
        /// </summary>
        /// <param name="channel">канал</param>
        /// <param name="state">состояние</param>
        /// <returns></returns>
        [DefModel(Model.PACE5000 | Model.PACE6000)]
        public bool SetOutputState(int channel, bool state)
        {
            Send(_parser.GetCommandSetOutputState(channel, state));
            return true;
            //TODO реализовать проверку установки состояния канала
        }

        /// <summary>
        /// Установить блокировку изменения режима управления пользователем
        /// </summary>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public void SetLocalLockOutMode()
        {
            Send(_parser.GetCommandSetLocalLockOutMode());
        }

        /// <summary>
        /// Снять блокировку изменения режима управления пользователем
        /// </summary>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public void SetOffLocalLockOutMode()
        {
            Send(_parser.GetCommandSetOffLocalLockOutMode());
        }

        /// <summary>
        /// Переключить в ручной режим управления
        /// </summary>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public void SetLocal()
        {
            Send(_parser.GetCommandSetLocal());
        }

        /// <summary>
        /// Переключить в режим удаленного управления
        /// </summary>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public void SetRemote()
        {
            Send(_parser.GetCommandSetRemote());
        }

        /// <summary>
        /// Получить количество точек калибровки PACE
        /// </summary>
        /// <returns></returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public int? GetCountCalibrationPoints()
        {
            Send(_parser.GetCommandGetNumCalibrPoints());
            string strReturn = Receive();
            int? result;
            if (!_parser.ParseGetNumCalibrPoints(strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Получить количество точек калибровки PACE по каналу
        /// </summary>
        /// <returns></returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public int? GetCountCalibrationPoints(int channel)
        {
            Send(_parser.GetCommandGetNumCalibrPoints(channel));
            string strReturn = Receive();
            int? result;
            if (!_parser.ParseGetNumCalibrPoints(strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Получить значение точки калибровки PACE по каналу
        /// </summary>
        /// <returns></returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public double? GetCalibrationPointValue(int channel, int index)
        {
            Send(_parser.GetCommandGetCalibPointsValue(channel, index));
            string strReturn = Receive();
            double? result;
            if (!_parser.ParseGetCalibPointsValue(strReturn, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Установить значение точки калибровки PACE по каналу
        /// </summary>
        /// <returns></returns>
        [DefModel(Model.PACE1000 | Model.PACE5000 | Model.PACE6000)]
        public void SetCalibrationPointValue(int channel, int index, double val)
        {
            Send(_parser.GetCommandSetCalibValueEntered(channel, index, val));
        }

        #region Service

        /// <summary>
        /// Обертка команды посылки сообщения
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool Send(string msg)
        {
            return _transport.Send(msg);
        }

        /// <summary>
        /// Обертка команды чтения ответа
        /// </summary>
        /// <returns></returns>
        private string Receive()
        {
            Thread.Sleep(50);
            return _transport.Receive();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Зарыть подключение
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

    }
}
