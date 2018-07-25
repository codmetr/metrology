using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PACESeries.Semantic;

namespace PACESeries
{
    /// <summary>
    /// Транслятор протокола PACE
    /// </summary>
    public class PACEParser
    {
        /*
         * :CALibration - calibration commands.
         * :DIAGnostic - instrument generated condition data.
         * INPut - switch input of the control module.
         * :INSTrument - instrument specific commands.
         * :OUTPut - controls the output pressure and logical outputs.
         * :SENSe - directs the instrument to measure selected parameters.
         * :SOURce - the commands that control the pressure outputs.
         * :STATus - instrument state.
         * :SYSTem - errors and SCPI version.
         * :UNIT - sets the units for the instrument.
         * Common SCPI commands - three letter commands, prefixed by *.
         * Instrument control commands - three letter commands, prefixed by :.
         */

        #region Keys
        internal const string KeyGetIdentificator = "*IDN?";
        internal const string KeySetOffLocalLockOut = ":GTL";
        internal const string KeySetLocalLockOut = ":LLO";
        internal const string KeySetLocal = ":LOC";
        internal const string KeySetRemote = ":REM";
        internal const string KeyGetDate = ":SYST:DATE?";
        internal const string KeyGetTime = ":SYST:TIME?";
        internal const string KeyGetUnitPressure = ":UNIT:PRES?";
        internal const string KeySetUnitPressure = ":UNIT:PRES <units>";
        internal const string KeyGetPressure = ":SENS:PRES?";
        internal const string KeyGetPressureByChannelFormat = ":SENS{0}:PRES?";
        internal const string KeySetPressureRangeFormat = ":SENS:PRES:RANG {0}";
        internal const string KeySetPressureRangeByChannelFormat = ":SENS{0}:PRES:RANG {1}";
        internal const string KeyGetPressureRange = ":SENS:PRES:RANG?";
        internal const string KeyGetPressureRangeByChannelFormat = ":SENS{0}:PRES:RANG?";
        internal const string KeyGetAvailableRange = ":INST:CAT:ALL?";
        internal const string KeyGetAvailableRangeByChannelFormat = ":INST:CAT{0}:ALL?";
        internal const string KeySetOutputStateFormat = ":OUTP:STAT{0} <state>";
        internal const string KeyGetOutputStateFormat = ":OUTP:STAT{0}?";
        internal const string KeySetOutputLogicalFormat = ":OUTP:LOG{0} <state>";
        internal const string KeyGetOutputLogicalFormat = ":OUTP:LOG{0}?";

        internal const string KeyGetNumCalibrPoints = ":CAL:PRES:POIN?";
        internal const string KeyGetNumCalibrPointsFormat = ":CAL{0}:PRES:POIN?";
        internal const string KeySetAcceptsCalibValues = ":CAL:PRES:ACC";
        internal const string KeySetAcceptsCalibValuesFormat = ":CAL{0}:PRES:ACC";
        internal const string KeySetAbortsCalibValues = ":CAL:PRES:ABOR";
        internal const string KeySetEnabCalibValueEntered = ":CAL:PRES:VAL";
        internal const string KeySetEnabCalibValueEnteredFormat = ":CAL{0}:PRES:VAL{1}";
        internal const string KeyGetCalibPointsValue = ":CAL:PRES:VAL?";
        internal const string KeyGetCalibPointsValueFormat = ":CAL{0}:PRES:VAL{1}?";

        // not applyed
        internal const string KeySetOpensClosesZeroValveFormat = ":CAL:PRES:ZERO:VALV {0}";
        internal const string KeyGetOpensClosesZeroValve = ":CAL:PRES:ZERO:VALV?";
        internal const string KeySetPressureZeroingFormat = ":CAL:PRES:ZERO:AUTO {0}";
        internal const string KeyGetPressureZeroing = ":CAL:PRES:ZERO:AUTO?";
        internal const string KeySetsTimedZeroInHours = ":CAL:PRES:ZERO:TIME";
        internal const string KeyGetTimedZeroInHours = ":CAL:PRES:ZERO:TIME?";
        internal const string KeySetsTimedZeroOnOff = ":CAL:PRES:ZERO:TIME:STAT";
        internal const string KeyGetsTimedZeroOnOff = ":CAL:PRES:ZERO:TIME:STAT?";


        #endregion

        #region GetIdentificator "*IDN?"
        /// <summary>
        /// Получить команду "Запрос идентификатора"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetIdentificator()
        {
            return KeyGetIdentificator;
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
            {
                {"Idn", PeremeterTypes.String}
            });
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
            return KeyGetDate;
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
            {
                {"year", PeremeterTypes.Integer},
                {"month", PeremeterTypes.Integer},
                {"day", PeremeterTypes.Integer}
            });
            year = (int)answer["year"];
            month = (int)answer["month"];
            day = (int)answer["day"];
            return true;
        }
        #endregion

        #region GetDate ":SYST:TIME?"
        /// <summary>
        /// Получить команду "Запрос текущего времени"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetTime()
        {
            return KeyGetTime;
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
        #endregion

        #region SetLocalLockOutMode ":LLO"
        /// <summary>
        /// Получить команду "Установка режима LLO"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetLocalLockOutMode()
        {
            return KeySetLocalLockOut;
        }
        #endregion

        #region SetOffLocalLockOutMode ":GTL"
        /// <summary>
        /// Получить команду "Установка режима LLO"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetOffLocalLockOutMode()
        {
            return KeySetOffLocalLockOut;
        }
        #endregion
         
        #region SetLocal ":LOC"
        /// <summary>
        /// Получить команду "Установка режима LOC"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetLocal()
        {
            return KeySetLocal;
        }
        #endregion

        #region SetRemote ":REM"
        /// <summary>
        /// Получить команду "Установка режима REM"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandSetRemote()
        {
            return KeySetRemote;
        }
        #endregion

        #region GetPressureUnit ":UNIT:PRES?"
        /// <summary>
        /// Получить команду "Запрос единиц давления"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetPressureUnit()
        {
            return KeyGetUnitPressure;
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
            return PaceSemanticRule.CompilCommand(KeySetUnitPressure, new Dictionary<string, string>() { { "<units>", unitstr } });
        }
        #endregion

        #region GetPressure ":SENS:PRES?"
        /// <summary>
        /// Получить команду "Измерение давления"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetPressure()
        {
            return KeyGetPressure;
        }

        /// <summary>
        /// Получить команду "Измерение давления по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetPressure(int channel)
        {
            return string.Format(KeyGetPressureByChannelFormat, channel);
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
            return KeyGetPressureRange;
        }

        /// <summary>
        /// Получить команду "Получение ограничение канала давления по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetPressureRange(int channel)
        {
            return string.Format(KeyGetPressureRangeByChannelFormat, channel);
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
            return string.Format(KeySetPressureRangeFormat, range);
        }

        /// <summary>
        /// Получить команду "Установку ограничение канала давления по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandSetPressureRange(int channel, string range)
        {
            return string.Format(KeySetPressureRangeByChannelFormat, channel, range);
        }
        #endregion

        #region GetAllRanges ":INST:CAT:ALL?"
        /// <summary>
        /// Получить команду "Получение всех допустимых ограничений канала давления"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetAllRanges()
        {
            return KeyGetAvailableRange;
        }

        /// <summary>
        /// Получить команду "Получение всех допустимых ограничений канала давления по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetAllRanges(int channel)
        {
            return string.Format(KeyGetAvailableRangeByChannelFormat, channel);
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
            return PaceSemanticRule.CompilCommand(string.Format(KeySetOutputStateFormat, ""),
                new Dictionary<string, string>() {{"<state>", state ? "ON" : "OFF"}});
        }

        /// <summary>
        /// Получить команду "Установить состояние выхода по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandSetOutputState(int channel, bool state)
        {
            return PaceSemanticRule.CompilCommand(string.Format(KeySetOutputStateFormat, channel),
                new Dictionary<string, string>() {{"<state>", state ? "ON" : "OFF"}});
        }

        #endregion

        #region GetOutputState ":OUTP:STAT?"
        /// <summary>
        /// Получить команду "Состояние выхода"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetOutputState()
        {
            return string.Format(KeyGetOutputStateFormat, "");
        }

        /// <summary>
        /// Получить команду "Состояние выхода по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetOutputState(int channel)
        {
            return string.Format(KeyGetPressureByChannelFormat, channel);
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
            return PaceSemanticRule.CompilCommand(string.Format(KeySetOutputLogicalFormat, ""),
                new Dictionary<string, string>() {{"<state>", state ? "ON" : "OFF"}});
        }

        /// <summary>
        /// Получить команду "Установить состояние реле по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandSetLogicState(int channel, bool state)
        {
            return PaceSemanticRule.CompilCommand(string.Format(KeySetOutputLogicalFormat, channel),
                new Dictionary<string, string>() {{"<state>", state ? "ON" : "OFF"}});
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
            return string.Format(KeyGetOutputLogicalFormat, "");
        }

        /// <summary>
        /// Получить команду "Получить состояние реле по каналу"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandGetLogicState(int channel, bool state)
        {
            return string.Format(KeyGetOutputLogicalFormat, channel);
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
            return KeyGetNumCalibrPoints;
        }

        /// <summary>
        /// Получить команду "Получение поличества точек калибровки"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandGetNumCalibrPoints(int channel)
        {
            return string.Format(KeyGetNumCalibrPointsFormat, channel);
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
            var answer = PaceSemanticRule.ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { {"num", PeremeterTypes.Integer} });
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
            return KeySetAcceptsCalibValues;
        }

        /// <summary>
        /// Получить команду "Подтверждение результата калибровки"
        /// </summary>
        /// <param name="channel">Канал</param>
        /// <returns>Команда</returns>
        public string GetCommandAcceptsCalibValues(int channel)
        {
            return string.Format(KeySetAcceptsCalibValuesFormat, channel);
        }
        #endregion

        #region Aborts calibration values ":CAL:PRES:ABOR"
        /// <summary>
        /// Получить команду "Отмена результата калибровки"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandAbortsCalibValues()
        {
            return KeySetAbortsCalibValues;
        }
        #endregion

        #region Enables calibration value to be entered ":CAL:PRES:VAL"
        /// <summary>
        /// Получить команду "Внести результат калибровки точки"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandEnableCalibValueEntered()
        {
            return KeySetEnabCalibValueEntered;
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
            return string.Format(KeySetEnabCalibValueEnteredFormat, channel, point);
        }
        #endregion

        #region Queries calibration point y value of module x ":CAL:PRES:VAL?"
        /// <summary>
        /// Получить команду "Запросить результат калибровки точки"
        /// </summary>
        /// <returns>Команда</returns>
        public string GetCommandGetCalibPointsValue()
        {
            return KeyGetCalibPointsValue;
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
            return string.Format(KeyGetCalibPointsValueFormat, channel, point);
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
