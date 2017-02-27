using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PACESeries.Semantic;
using static PACESeries.PACEParserV2;

namespace PACESeries
{
    /// <summary>
    /// Транслятор протокола PACE
    /// </summary>
    public class PACEParserV2
    {
        #region GetIdentificator "*IDN?"
        /// <summary>
        /// Получить команду "Запрос идентификатора"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetIdentificator()
        {
            return new LexNode<Codes>(Codes.IDN).Get();
        }

        /// <summary>
        /// Разобрать ответ на команду "Запрос идентификатора"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="idn">Идентификатор</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetIdentificator(string message, out string idn)
        {
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
                {{"Idn", PeremeterTypes.String}});
            idn = (string)answer["Idn"];
            return true;
        }
        #endregion

        #region GetDate ":SYST:DATE?"
        /// <summary>
        /// Получить команду "Запрос текущей даты"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetDate()
        {
            return new LexNode<Codes>(Codes.SYST).Add(Codes.DATE).Get();
        }

        /// <summary>
        /// Разобрать ответ на команду "Запрос текущей даты"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="year">год</param>
        /// <param name="month">месяц</param>
        /// <param name="day">день</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetDate(string message, out int year, out int month, out int day)
        {
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
                {{"year", PeremeterTypes.Integer},
                {"month", PeremeterTypes.Integer},
                {"day", PeremeterTypes.Integer}});
            year = (int)answer["year"];
            month = (int)answer["month"];
            day = (int)answer["day"];
            return true;
        }

        /// <summary>
        /// Разобрать ответ на команду "Запрос текущей даты"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="date">Дата</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetDate(string message, out DateTime date)
        {
            int year;
            int month;
            int day;
            var answer = ParseGetDate(message, out year, out month, out day);
            date = new DateTime(year, month, day);
            return answer;
        }
        #endregion


        #region GetDate ":SYST:TIME?"
        /// <summary>
        /// Получить команду "Запрос текущего времени"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetTime()
        {
            return new LexNode<Codes>(Codes.SYST).Add(Codes.TIME).Get();
        }

        /// <summary>
        /// Разобрать ответ на команду "Запрос текущего времени"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="hour">час</param>
        /// <param name="minute">минута</param>
        /// <param name="sec">секунда</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetTime(string message, out int hour, out int minute, out int sec)
        {
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
            {
                {"hour", PeremeterTypes.Integer},
                {"month", PeremeterTypes.Integer},
                {"day", PeremeterTypes.Integer}
            });
            hour = (int)answer["hour"];
            minute = (int)answer["minute"];
            sec = (int)answer["sec"];
            return true;
        }

        /// <summary>
        /// Разобрать ответ на команду "Запрос текущего времени"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="time"></param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetTime(string message, out DateTime time)
        {
            int hour;
            int min;
            int sec;
            var answer = ParseGetTime(message, out hour, out min, out sec);
            time = new DateTime(0,0,0, hour, min, sec);
            return answer;
        }
        #endregion

        #region SetLocalLockOutMode ":LLO"
        /// <summary>
        /// Получить команду "Установка режима LLO"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetLocalLockOutMode()
        {
            return new LexNode<Codes>(Codes.LLO).Set();
        }
        #endregion

        #region SetOffLocalLockOutMode ":GTL"
        /// <summary>
        /// Получить команду "Установка режима GTL"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetOffLocalLockOutMode()
        {
            return new LexNode<Codes>(Codes.GTL).Set();
        }
        #endregion

        #region SetLocal ":LOC"
        /// <summary>
        /// Получить команду "Установка режима LOC"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetLocal()
        {
            return new LexNode<Codes>(Codes.LOC).Set();
        }
        #endregion

        #region SetRemote ":REM"
        /// <summary>
        /// Получить команду "Установка режима REM"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetRemote()
        {
            return new LexNode<Codes>(Codes.REM).Set();
        }
        #endregion

        #region GetPressureUnit ":UNIT:PRES?"
        /// <summary>
        /// Получить команду "Запрос единиц давления"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetPressureUnit()
        {
            return new LexNode<Codes>(Codes.UNIT).Add(Codes.PRESsure).Get();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="unit"></param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetPressureUnit(string message, out PressureUnits? unit)
        {
            unit = null;
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "unit", PeremeterTypes.PressureUnit } });
            unit = (PressureUnits)answer["unit"];
            return true;
        }
        #endregion

        #region SetPressureUnit "UNIT:PRES <units>"
        /// <summary>
        /// Получить команду "Установить единицы давления"
        /// </summary>
        /// <param name="unit">единицы давления</param>
        /// <returns>Команда</returns>
        public string GetCommandSetPressureUnit(PressureUnits unit)
        {
            string unitstr = null;
            if (unit == PressureUnits.None)
                throw new Exception(string.Format("can not set pressure unit PressureUnits.None"));
            unitstr = PaceSemanticRule.PressureUnitToString(unit);
            return new LexNode<Codes>(Codes.UNIT).Add(Codes.PRESsure).Set(unitstr);
            //return PaceSemanticRule.CompilCommand(KeySetUnitPressure, new Dictionary<string, string>() { { "<units>", unitstr } });
        }
        #endregion

        #region GetPressure ":SENS:PRES?"
        /// <summary>
        /// Получить команду "Измерение давления"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetPressure()
        {
            return new LexNode<Codes>(Codes.SENS).Add(Codes.PRESsure).Get();
        }

        /// <summary>
        /// Получить команду "Измерение давления по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetPressure(int channel)
        {
            return new LexNode<Codes>(Codes.SENS).Add(channel).Add(Codes.PRESsure).Get();
        }

        /// <summary>
        /// Разобрать результат команды "Измерение давления"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="value">Значение давления</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetPressure(string message, out double? value)
        {
            value = null;
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.Real } });
            value = (double)answer["value"];
            return true;
        }
        #endregion

        #region GetPressureRange ":SENS:PRES:RANG?"
        /// <summary>
        /// Получить команду "Получение ограничение канала давления"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetPressureRange()
        {
            return new LexNode<Codes>(Codes.SENS).Add(Codes.PRESsure).Add(Codes.RANG).Get();
        }

        /// <summary>
        /// Получить команду "Получение ограничение канала давления по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetPressureRange(int channel)
        {
            return new LexNode<Codes>(Codes.SENS).Add(channel).Add(Codes.PRESsure).Add(Codes.RANG).Get();
        }

        /// <summary>
        /// Разобрать результат команды "Получение ограничение канала давления"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="value">Ограничение</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetPressureRange(string message, out string value)
        {
            value = null;
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.String } });
            value = (string)answer["value"];
            return true;
        }
        #endregion

        #region SetPressureRange ":SENS:PRES:RANG"
        /// <summary>
        /// Получить команду "Установку ограничение канала давления"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetPressureRange(string range)
        {
            return new LexNode<Codes>(Codes.SENS).Add(Codes.PRESsure).Add(Codes.RANG).Set(range);
        }

        /// <summary>
        /// Получить команду "Установку ограничение канала давления по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandSetPressureRange(int channel, string range)
        {
            return new LexNode<Codes>(Codes.SENS).Add(channel).Add(Codes.PRESsure).Add(Codes.RANG).Set(range);
        }
        #endregion

        #region GetAllRanges ":INST:CAT:ALL?"
        /// <summary>
        /// Получить команду "Получение всех допустимых ограничений канала давления"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetAllRanges()
        {
            return new LexNode<Codes>(Codes.INST).Add(Codes.CAT).Add(Codes.ALL).Get();
        }

        /// <summary>
        /// Получить команду "Получение всех допустимых ограничений канала давления по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetAllRanges(int channel)
        {
            return new LexNode<Codes>(Codes.INST).Add(Codes.CAT).Add(channel).Add(Codes.ALL).Get();
        }

        /// <summary>
        /// Разобрать результат команды "Получение ограничение каналов давления"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="value">Список ограничений</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetAllRanges(string message, out IEnumerable<string> value)
        {
            value = null;
            var answer = PaceSemanticRule.ParseAnswer(message);
            value = answer;
            return true;
        }
        #endregion

        #region SetOutputState ":OUTP:STAT <state>"

        /// <summary>
        /// Получить команду "Установить состояние выхода по каналу"
        /// </summary>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandSetOutputState(bool state)
        {
            return new LexNode<Codes>(Codes.OUTP).Add(Codes.STAT).Set(state ? "ON" : "OFF");
        }

        /// <summary>
        /// Получить команду "Установить состояние выхода по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandSetOutputState(int channel, bool state)
        {
            return new LexNode<Codes>(Codes.OUTP).Add(Codes.STAT).Add(channel).Set(state ? "ON" : "OFF");
        }

        #endregion

        #region GetOutputState ":OUTP:STAT?"
        /// <summary>
        /// Получить команду "Состояние выхода"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetOutputState()
        {
            return new LexNode<Codes>(Codes.OUTP).Add(Codes.STAT).Get();
        }

        /// <summary>
        /// Получить команду "Состояние выхода по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetOutputState(int channel)
        {
            return new LexNode<Codes>(Codes.OUTP).Add(Codes.STAT).Add(channel).Get();
        }

        /// <summary>
        /// Разобрать результат команды "Состояние выхода"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="value">Состояние выхода</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetOutputState(string message, out bool? value)
        {
            value = null;
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.Boolean } });
            value = (bool)answer["value"];
            return true;
        }
        #endregion

        #region SetLogicState ":OUTP:LOG <state>"

        /// <summary>
        /// Получить команду "Установить состояние реле по каналу по умолчанию"
        /// </summary>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandSetLogicState(bool state)
        {
            return new LexNode<Codes>(Codes.OUTP).Add(Codes.LOG).Set(state ? "ON" : "OFF");
        }

        /// <summary>
        /// Получить команду "Установить состояние реле по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandSetLogicState(int channel, bool state)
        {
            return new LexNode<Codes>(Codes.OUTP).Add(Codes.LOG).Add(channel).Set(state ? "ON" : "OFF");
        }
        #endregion

        #region GetLogicState ":OUTP:LOG?"

        /// <summary>
        /// Получить команду "Получить состояние реле по каналу по умолчанию"
        /// </summary>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandGetLogicState(bool state)
        {
            return new LexNode<Codes>(Codes.OUTP).Add(Codes.LOG).Get();
        }

        /// <summary>
        /// Получить команду "Получить состояние реле по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandGetLogicState(int channel, bool state)
        {
            return new LexNode<Codes>(Codes.OUTP).Add(Codes.LOG).Add(channel).Get();
        }

        /// <summary>
        /// Разобрать результат команды "Получить состояние реле по каналу"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="value">Состояние выхода</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetLogicState(string message, out bool? value)
        {
            value = null;
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.Boolean } });
            value = (bool)answer["value"];
            return true;
        }
        #endregion

        #region Gets the number of calibration points ":CAL:PRES:POIN?"
        /// <summary>
        /// Получить команду "Получение поличества точек калибровки"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetNumCalibrPoints()
        {
            return new LexNode<Codes>(Codes.CAL).Add(Codes.PRESsure).Add(Codes.POIN).Get();
        }

        /// <summary>
        /// Получить команду "Получение поличества точек калибровки"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetNumCalibrPoints(int channel)
        {
            return new LexNode<Codes>(Codes.CAL).Add(Codes.PRESsure).Add(Codes.POIN).Add(channel).Get();
        }

        /// <summary>
        /// Разобрать результат команды "Получение поличества точек калибровки"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="value">Список ограничений</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetNumCalibrPoints(string message, out int? value)
        {
            value = null;
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "num", PeremeterTypes.Integer } });
            value = (int)answer["num"];
            return true;
        }
        #endregion

        #region Accepts calibration values ":CAL:PRES:ACC"
        /// <summary>
        /// Получить команду "Подтверждение результата калибровки"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandAcceptsCalibValues()
        {
            return new LexNode<Codes>(Codes.CAL).Add(Codes.PRESsure).Add(Codes.ACC).Set();
        }

        /// <summary>
        /// Получить команду "Подтверждение результата калибровки"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandAcceptsCalibValues(int channel)
        {
            return new LexNode<Codes>(Codes.CAL).Add(Codes.PRESsure).Add(Codes.ACC).Add(channel).Set();
        }
        #endregion

        #region Aborts calibration values ":CAL:PRES:ABOR"
        /// <summary>
        /// Получить команду "Отмена результата калибровки"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandAbortsCalibValues()
        {
            return new LexNode<Codes>(Codes.CAL).Add(Codes.PRESsure).Add(Codes.ABOR).Set();
        }
        #endregion

        #region Enables calibration value to be entered ":CAL:PRES:VAL"
        /// <summary>
        /// Получить команду "Внести результат калибровки точки"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandEnableCalibValueEntered()
        {
            return new LexNode<Codes>(Codes.CAL).Add(Codes.PRESsure).Add(Codes.VAL).Set();
        }

        /// <summary>
        /// Получить команду "Внести результат калибровки точки"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="point">Точка калибровки
        /// 1 - lower pressure
        /// 2 - middle pressure
        /// 3 - higher pressure</param>
        /// <returns>Команда</returns>
        public string GetCommandEnableCalibValueEntered(int channel, int point)
        {
            return new LexNode<Codes>(Codes.CAL).Add(channel).Add(Codes.PRESsure).Add(Codes.VAL).Add(point).Set();
        }
        #endregion

        #region Queries calibration point y value of module x ":CAL:PRES:VAL?"
        /// <summary>
        /// Получить команду "Запросить результат калибровки точки"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetCalibPointsValue()
        {
            return new LexNode<Codes>(Codes.CAL).Add(Codes.PRESsure).Add(Codes.VAL).Get();
        }

        /// <summary>
        /// Получить команду "Запросить результат калибровки точки"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="point">Точка калибровки
        /// 1 - lower pressure
        /// 2 - middle pressure
        /// 3 - higher pressure</param>
        /// <returns>Команда</returns>
        public string GetCommandGetCalibPointsValue(int channel, int point)
        {
            return new LexNode<Codes>(Codes.CAL).Add(channel).Add(Codes.PRESsure).Add(Codes.VAL).Add(point).Get();
        }

        /// <summary>
        /// Разобрать результат команды "Запросить результат калибровки точки"
        /// </summary>
        /// <param name="message">Ответ</param>
        /// <param name="value">Результат калибровки точки</param>
        /// <returns>Удалось разобрать</returns>
        public bool ParseGetCalibPointsValue(string message, out int? value)
        {
            value = null;
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.Integer } });
            value = (int)answer["value"];
            return true;
        }
        #endregion
    }
}
