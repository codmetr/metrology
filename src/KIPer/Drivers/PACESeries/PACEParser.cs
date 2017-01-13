using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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
        internal const string KeySetOutputStateFormat = ":OUTP:SET{0} <state>";
        internal const string KeyGetOutputStateFormat = ":OUTP:SET{0}?";
        
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
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
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
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
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
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>()
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
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "unit", PeremeterTypes.PressureUnit } });
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
            unitstr = PressureUnitToString(unit);
            return CompilCommand(KeySetUnitPressure, new Dictionary<string, string>() { { "<units>", unitstr } });
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
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.Real } });
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
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.String } });
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
            var answer = ParseAnswer(message);
            value = answer;
            return true;
        }
        #endregion

        #region GetCommandSetOutputState ":OUTP:SET <state>"

        /// <summary>
        /// Получить команду "Установить состояние выхода по каналу"
        /// </summary>
        /// <param name="state">Состояние</param>
        /// <returns>Команда</returns>
        public string GetCommandSetOutputState(bool state)
        {
            string unitstr = null;
            return CompilCommand(string.Format(KeySetOutputStateFormat, ""),
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
            string unitstr = null;
            return CompilCommand(string.Format(KeySetOutputStateFormat, channel),
                new Dictionary<string, string>() {{"<state>", state ? "ON" : "OFF"}});
        }

        #endregion

        #region GetPressure ":OUTP:SET?"
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
            var answer = ParseAnswer(message, new Dictionary<string, PeremeterTypes>() { { "value", PeremeterTypes.Boolean } });
            value = (bool)answer["value"];
            return true;
        }
        #endregion
        
        #region Protocol parser
        /// <summary>
        /// Сбор команды
        /// </summary>
        /// <param name="frame">форма команды</param>
        /// <param name="parameters">набор параметров</param>
        /// <returns>команда</returns>
        private string CompilCommand(string frame, IDictionary<string, string> parameters)
        {
            string result = frame;
            if ((frame.Contains('<') || frame.Contains('>')) && (parameters == null || parameters.Count == 0))
                throw new Exception(string.Format("Try compil colmmand by frame[\"{0}\"] with parameters and not set parameters", frame));
            var countStartNameChar = frame.Count(el => el == '<');
            var countEndNameChar = frame.Count(el => el == '>');
            if (countStartNameChar < countEndNameChar)
                throw new Exception(string.Format("Frame[\"{0}\"] contains \'<\' simbols less then \'>\'", frame));
            if (countStartNameChar > countEndNameChar)
                throw new Exception(string.Format("Frame[\"{0}\"] contains \'<\' simbols more then \'>\'", frame));

            var parts = frame.Split(new[] { ' ', ',' });
            for (int i = 1; i < parts.Length; i++)
            {
                var key = parts[i];
                if (!parameters.ContainsKey(key))
                    throw new Exception(string.Format("Frame[\"{0}\"] contains not setted parameter:{1}", frame, key));
                result = result.Replace(string.Format("{0}", key), parameters[key]);
            }
            return result;
        }

        /// <summary>
        /// Разбор ответа
        /// </summary>
        /// <param name="answer">Ответ</param>
        /// <returns>Набор параметров</returns>
        private string[] ParseAnswer(string answer)
        {
            return answer.Trim().Split(' ')[1].Split(',');
        }

        /// <summary>
        /// Разбор ответа
        /// </summary>
        /// <param name="answer">Ответ</param>
        /// <param name="parameterTypes">Типы параметров</param>
        /// <returns>Набор разобранных параметров</returns>
        private IDictionary<string, object> ParseAnswer(string answer, Dictionary<string, PeremeterTypes> parameterTypes)
        {
            var result = new Dictionary<string, object>();
            var parts = ParseAnswer(answer);
            if (parts.Length != parameterTypes.Count)
                throw new Exception(string.Format("Count[{0}] parts answer[{1}] not equal count[{2}] parameters", parts.Length, answer, parameterTypes.Count));
            var keys = parameterTypes.Keys.ToList();
            for (int i = 0; i < parts.Length; i++)
            {
                var key = keys[i];
                var parameter = parts[i];
                parameter = parameter.Trim();
                switch (parameterTypes[key])
                {
                    case PeremeterTypes.Real:
                        double doubleVal;
                        if (!double.TryParse(parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleVal))
                            throw new Exception(string.Format("Can not cast \"{0}\" to double", parameter));
                        result.Add(key, doubleVal);
                        break;
                    case PeremeterTypes.Integer:
                        int intVal;
                        if (!int.TryParse(parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out intVal))
                            throw new Exception(string.Format("Can not cast \"{0}\" to int", parameter));
                        result.Add(key, intVal);
                        break;
                    case PeremeterTypes.Boolean:
                        bool boolVal;
                        if (parameter == "ON" || parameter == "1")
                            boolVal = true;
                        else if (parameter == "OFF" || parameter == "0")
                            boolVal = false;
                        else
                            throw new Exception(string.Format("Can not cast \"{0}\" to bool", parameter));
                        result.Add(key, boolVal);
                        break;
                    case PeremeterTypes.String:
                        result.Add(key, parameter);
                        break;
                    case PeremeterTypes.PressureUnit:
                        if (parameter == "MBAR")
                            result.Add(key, PressureUnits.MBar);
                        else if (parameter == "BAR")
                            result.Add(key, PressureUnits.Bar);
                        else if (parameter == "INH2O4")
                            result.Add(key, PressureUnits.inH2O4);
                        else if (parameter == "INH2O")
                            result.Add(key, PressureUnits.inH2O);
                        else if (parameter == "INHG")
                            result.Add(key, PressureUnits.inHg);
                        else if (parameter == "MMHG")
                            result.Add(key, PressureUnits.mmHg);
                        else if (parameter == "PA")
                            result.Add(key, PressureUnits.Pa);
                        else if (parameter == "HPA")
                            result.Add(key, PressureUnits.hPa);
                        else if (parameter == "KPA")
                            result.Add(key, PressureUnits.kPa);
                        else if (parameter == "PSI")
                            result.Add(key, PressureUnits.psi);
                        else if (parameter == "INH2O60")
                            result.Add(key, PressureUnits.inH2O60F);
                        else if (parameter == "KG/CM2")
                            result.Add(key, PressureUnits.KgCm2);
                        else if (parameter == "ATM")
                            result.Add(key, PressureUnits.ATM);
                        else if (parameter == "MMH2O4")
                            result.Add(key, PressureUnits.mmH2O4);
                        else
                            throw new Exception(string.Format("Can not cast \"{0}\" to PressureUnit", parameter));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return result;
        }

        /// <summary>
        /// Единицы измерения в строку
        /// </summary>
        /// <param name="unit">Единицы измерения давления</param>
        /// <returns>строковое представление единицы измерения давления</returns>
        private string PressureUnitToString(PressureUnits unit)
        {
            string unitstr = null;
            switch (unit)
            {
                case PressureUnits.None:
                    throw new Exception(string.Format("can not set pressure unit PressureUnits.None"));
                    break;
                case PressureUnits.MBar:
                    unitstr = "MBAR";
                    break;
                case PressureUnits.Bar:
                    unitstr = "BAR";
                    break;
                case PressureUnits.inH2O4:
                    unitstr = "INH2O4";
                    break;
                case PressureUnits.inH2O:
                    unitstr = "INH2O";
                    break;
                case PressureUnits.inHg:
                    unitstr = "INHG";
                    break;
                case PressureUnits.mmHg:
                    unitstr = "MMHG";
                    break;
                case PressureUnits.Pa:
                    unitstr = "PA";
                    break;
                case PressureUnits.hPa:
                    unitstr = "HPA";
                    break;
                case PressureUnits.kPa:
                    unitstr = "KPA";
                    break;
                case PressureUnits.psi:
                    unitstr = "PSI";
                    break;
                case PressureUnits.inH2O60F:
                    unitstr = "INH2O60";
                    break;
                case PressureUnits.KgCm2:
                    unitstr = "KG/CM2";
                    break;
                case PressureUnits.ATM:
                    unitstr = "ATM";
                    break;
                case PressureUnits.mmH2O4:
                    unitstr = "MMH2O4";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unit");
            }
            return unitstr;
        }
        #endregion

        #region helpers

        #endregion
    }
}
