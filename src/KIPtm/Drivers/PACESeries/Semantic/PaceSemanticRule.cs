using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PACESeries.Semantic
{
    static class PaceSemanticRule
    {
        #region Protocol parser
        /// <summary>
        /// Сбор команды
        /// </summary>
        /// <param name="frame">форма команды</param>
        /// <param name="parameters">набор параметров</param>
        /// <returns>команда</returns>
        public static string CompilCommand(string frame, IDictionary<string, string> parameters)
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
        public static string[] ParseAnswer(string answer)
        {
            return answer.Trim().Split(' ')[1].Split(',');
        }

        /// <summary>
        /// Разбор ответа
        /// </summary>
        /// <param name="answer">Ответ</param>
        /// <param name="parameterTypes">Типы параметров</param>
        /// <returns>Набор разобранных параметров</returns>
        public static IDictionary<string, object> ParseAnswer(string answer, Dictionary<string, PeremeterTypes> parameterTypes)
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
        public static string PressureUnitToString(PressureUnits unit)
        {
            string unitstr = null;
            switch (unit)
            {
                case PressureUnits.None:
                    throw new Exception(string.Format("can not set pressure unit PressureUnits.None"));
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
    }
}
